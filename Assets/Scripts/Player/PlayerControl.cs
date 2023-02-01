using System.Collections;
using System.Net.Sockets;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// The position origins of the raycasts used to keep the player in bounds
/// </summary>
public struct RayCastOrigins {
    public Vector2 topLeft, topRight, bottomLeft, bottomRight;
}

/// <summary>
/// Which directions the player is colliding with a solid collider
/// </summary>
public struct CollisionDirections {
    public bool up, down, right, left;
    public void Reset() {
        up = down = right = left = false;
    }
}

/// <summary>
/// Which direction the player is facing; used to determine if they can interact with an object
/// </summary>
public enum FacingDirection {
    Up, 
    Down, 
    Left, 
    Right
}

/// <summary>
/// The possible states the player can be in
/// </summary>
public enum PlayerState {
    Idle,
    Attack,
    Dashing
}

public class PlayerControl : MonoBehaviour {

    [SerializeField]
    private bool showDebug = true;

    private Animator animator;
    [SerializeField]
    private GameObject afterImage;

    private PlayerState playerState = PlayerState.Idle;

    #region Movement
    //movement
    private bool canMove = true;
    public bool CanMove { get => canMove; set { canMove = value; } }
    private const float walkSpeed = 5f;
    //We use a separate input vector to be able to continously collect the player's input, even if movement is disabled
    private Vector2 input;
    private Vector2 velocity;
    private float focusScalar = 1f;
    private FacingDirection facingDirection = FacingDirection.Down;

    //dashing
    private bool canDash = true;
    private const int maxDashCharges = 2;
    private const float dashSpeed = 20f;
    private int currentDashCharges = maxDashCharges;
    //how long the player is dashing for in seconds
    private float dashLength = 0.12f;
    //how long it takes for a dash charge to come back, starting when the dash ends
    private float dashRechargeTime = 2f;

    //active move speed is changed between the value of walkSpeed and dashSpeed when the player starts or ends a dash
    //by default is is equal to walkSpeed
    private float activeMoveSpeed = walkSpeed;
    #endregion

    #region Attack Info
    [SerializeField]
    private AnimationClip attackAnimation;
    [SerializeField]
    private int damageFrame;
    private int totalAttackFrames;
    private float attackSpeed = 3f;
    public GameObject hitbox;
    #endregion

    #region Raycasting & Collisions
    [SerializeField]
    //the layers that the player will collide with the environment
    private LayerMask environmentLayers;
    private new BoxCollider2D collider;
    private Bounds bounds => collider.bounds;
    private CollisionDirections collisionDirs;
    //The inner corners of the player's hitbox that raycasts start from
    private RayCastOrigins rayOrigins;
    const float skinWidth = 0.015f;
    const int horizontalRayCount = 3;
    const int verticalRayCount = 3;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    public FacingDirection FacingDirection {
        get {return facingDirection;}
    }

    #endregion

    private void OnDrawGizmos() {
        //Gizmos.DrawWireCube(bounds.center, bounds.size);
        #region Raycasts

        #endregion
    }

    /// <summary>
    /// Draws debug info to the screen
    /// </summary>
    private void OnGUI() {
        if (showDebug) {
            int yStart = 5;
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"activeMoveSpeed: {activeMoveSpeed}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"dashCharges: {currentDashCharges}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"playerState: {playerState}");

            GUI.Label(new Rect(5, yStart += 30, 300, 150), $"up collision: {collisionDirs.up}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"right collision: {collisionDirs.right}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"down collision: {collisionDirs.down}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"left collision: {collisionDirs.left}");

            GUI.Label(new Rect(5, yStart += 30, 300, 150), $"facing direction: {facingDirection}");

            GUI.Label(new Rect(5, yStart += 30, 300, 150), $"health: {PlayerInfo.Instance.Health}");
        }
    }

    /// <summary>
    /// Runs when the object is enabled for the first time
    /// </summary>
    void Start() { 
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();

        collisionDirs = new CollisionDirections();
        rayOrigins = new RayCastOrigins();
        CalculateRaySpacing();
        UpdateRayCastOrigins();

        totalAttackFrames = (int)(attackAnimation.length * attackAnimation.frameRate);
    }

    /// <summary>
    /// Runs every frame (variable depending on hardware)
    /// </summary>
    void Update() {
        animator.SetBool("IsAttacking", playerState == PlayerState.Attack);
        switch (playerState) {
            case PlayerState.Idle:
                if (velocity != Vector2.zero) {
                    animator.SetFloat("Horizontal", velocity.x);
                    animator.SetFloat("Vertical", velocity.y);
                }
                animator.SetFloat("Speed", velocity.sqrMagnitude);

                //sets the player's facing direction based on their velocity
                if (velocity.y > 0)
                    facingDirection = FacingDirection.Up;
                else if (velocity.y < 0)
                    facingDirection = FacingDirection.Down;
                else if (velocity.x > 0)
                    facingDirection = FacingDirection.Right;
                else if (velocity.x < 0)
                    facingDirection = FacingDirection.Left;
                break;
        }

        if (GameObject.FindGameObjectWithTag("Bullet") != null) {
            hitbox.SetActive(true);
        } else {
            hitbox.SetActive(false);
        }
    }

    /// <summary>
    /// Runs every physics frame (default is 50 times a second)
    /// </summary>
    void FixedUpdate() {
        //stops the player when they run into a wall, leave commented if you want the player to continue their run animation against a wall
        /*if (collisionDirs.down || collisionDirs.up) {
            velocity.y = 0;
        }
        if (collisionDirs.left || collisionDirs.right) {
            velocity.x = 0;
        }*/

        UpdateRayCastOrigins();
        collisionDirs.Reset();

        //put any scalar values before velocity for better performance
        Vector2 newVel = activeMoveSpeed * focusScalar * Time.fixedDeltaTime * velocity;

        //if the player is moving vertically or horizontally, check for collisions in those directions
        if (velocity.x != 0) {
            HorizontalCollisions(ref newVel);
        }
        if (velocity.y != 0) {
            VerticalCollisions(ref newVel);
        }

        //move the player by the (possibly) corrected velocity
        transform.Translate(newVel);
    }

    #region Collisions & Raycasts
    /// <summary>
    /// Calculates the spacing of the raycasts used for player collision
    /// This is only run once: at startup
    /// </summary>
    private void CalculateRaySpacing() {
        //We make a copy of the bounds so that we can expand it without affect the player's actual hitbox
        Bounds tempBounds = bounds;
        tempBounds.Expand(skinWidth * -2);

        horizontalRaySpacing = tempBounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = tempBounds.size.x / (verticalRayCount - 1);
    }

    /// <summary>
    /// Updates the world position of the raycasts in the 
    /// </summary>
    private void UpdateRayCastOrigins() {
        Bounds tempBounds = bounds;
        tempBounds.Expand(skinWidth * -2);

        rayOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        rayOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    /// <summary>
    /// Determines if the player will collide with an object in the upleft/right direction
    /// If a collision were to take place, adjusts the player's velocity so that they will not
    /// </summary>
    /// <param name="velocity">reference to the player's current velocity</param>
    private void HorizontalCollisions(ref Vector2 velocity) {
        //left is negative, right is positive
        float directionX = Mathf.Sign(velocity.x);
        //length of the ray is scaled by the player's velocity plus the skinWidth
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
            //depending on which way the player is moving, change the starting corner to raycast from
            Vector2 rayOrigin = (directionX == -1) ? rayOrigins.bottomLeft : rayOrigins.bottomRight;
            //adjust the starting position based on iteration
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, environmentLayers);

            //Debug.DrawRay(rayOrigin, directionX * rayLength * Vector2.right, Color.blue);

            //if the ray cast hits, change the velocity so the player will not end up in the object and set the collision direction
            if (rayHit) {
                velocity.x = (rayHit.distance - skinWidth) * directionX;
                //prevents rays past the first one from override the velocity adjustment if they would hit
                rayLength = rayHit.distance;

                //set collision direction based on movement direction
                collisionDirs.right = !(collisionDirs.left = directionX == -1);
            }
        }
    }

    /// <summary>
    /// Determines if the player will collide with an object in the up/down direction
    /// If a collision were to take place, adjusts the player's velocity so that they will not
    /// </summary>
    /// <param name="velocity">reference to the player's current velocity</param>
    private void VerticalCollisions(ref Vector2 velocity) {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1) ? rayOrigins.bottomLeft : rayOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, environmentLayers);

            Debug.DrawRay(rayOrigin, directionY * rayLength * Vector2.up, Color.blue);

            if (rayHit) {
                velocity.y = (rayHit.distance - skinWidth) * directionY;
                //prevents rays past the first one from override the velocity adjustment if they would hit
                rayLength = rayHit.distance;

                //set collision direction based on movement direction
                collisionDirs.up = !(collisionDirs.down = directionY == -1);
            }
        }
    }
    #endregion

    #region Input
    /// <summary>
    /// Interprets the player's input for movement
    /// </summary>
    /// <param name="context"></param>
    public void Move(InputAction.CallbackContext context) {
        input = context.ReadValue<Vector2>().normalized;
        if (canMove) {
            velocity = input;
        }
    }

    #region Interact

    public void Interact(InputAction.CallbackContext context) {
        if (context.performed && PlayerInfo.Instance.CanInteract) {
            PlayerInfo.Instance.Interactable.OnInteract();
        }
    }

    #endregion

    #region Attack
    /// <summary>
    /// Interprets the player's input for attacking
    /// </summary>
    /// <param name="context"></param>
    public void Attack(InputAction.CallbackContext context) {
        if (context.performed && playerState != PlayerState.Attack) {
            playerState = PlayerState.Attack;
            //ensures the player is set to walking speed if they attack cancel a dash
            activeMoveSpeed = attackSpeed;
            //disables dashing so the player cannot dash cancel until the attack boxcast has been done
            //and disables movement so the player cannot change their direction
            canDash = false;
            canMove = false;
            StartCoroutine(EndAttack());
        }
    }

    /// <summary>
    /// Waits for the damage frame of the attack animation then re-enables dashing and performs a boxcast for any enemies in front of the player
    /// After the full length of the attack animation sets the player state back to idle
    /// </summary>
    /// <returns></returns>
    IEnumerator EndAttack() {
        yield return new WaitForSeconds((float)damageFrame / totalAttackFrames * attackAnimation.length);
        canDash = true;

        //TODO: boxcast for attacking

        yield return new WaitForSeconds((1.0f - (float)damageFrame / totalAttackFrames) * attackAnimation.length);
        playerState = PlayerState.Idle;
        activeMoveSpeed = walkSpeed;
        canMove = true;
        velocity = input;
    }
    #endregion

    #region Dash
    /// <summary>
    /// Interprets the player's input for dashing
    /// </summary>
    /// <param name="context"></param>
    public void Dash(InputAction.CallbackContext context) {
        if (context.performed && currentDashCharges > 0 && playerState != PlayerState.Dashing && canDash) {
            playerState = PlayerState.Dashing;
            activeMoveSpeed = dashSpeed;
            currentDashCharges--;
            StartCoroutine(EndDash());
            StartCoroutine(CreateAfterImages());
        }
    }

    /// <summary>
    /// Ends the player's dash after the set time by setting their state to idle and setting the activeMoveSpeed to walkSpeed
    /// Waits the set amount of time before recharging the used dash charge
    /// </summary>
    /// <returns></returns>
    IEnumerator EndDash() {
        yield return new WaitForSeconds(dashLength);
        //if the player ends dashing in the dash state, change back to idle state
        if (playerState == PlayerState.Dashing) {
            playerState = PlayerState.Idle;
            activeMoveSpeed = walkSpeed;
        //if the player ends dashing in the attack state, keep the reduced attack speed
        } else if (playerState == PlayerState.Attack) {
            activeMoveSpeed = attackSpeed;
        }

        yield return new WaitForSeconds(dashRechargeTime);
        currentDashCharges++;
    }

    /// <summary>
    /// Creates the afterimage clones of the player while dashing
    /// Every other physics frame a clone is created
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateAfterImages() {
        while (playerState == PlayerState.Dashing) {
            GameObject echoInstance = Instantiate(afterImage, transform.position, Quaternion.identity);
            echoInstance.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime * 2);
        }
    }
    #endregion

    #region Focus
    /// <summary>
    /// Interprets the player's input for focus mode
    /// </summary>
    /// <param name="context"></param>
    public void Focus(InputAction.CallbackContext context) {
        if (context.performed) {
            focusScalar = 0.7f;
        } else if (context.canceled) {
            focusScalar = 1f;
        }
    }
    #endregion
    #endregion

    /// <summary>
    /// Zeros the player's current velocity and disables their movement
    /// </summary>
    public void Freeze() {
        canMove = false;
        velocity = Vector2.zero;
    }
}