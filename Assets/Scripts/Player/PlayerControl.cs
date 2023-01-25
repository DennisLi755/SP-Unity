using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

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
public struct FacingDirection {
    bool up, down, left, right;
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
    private const float walkSpeed = 5f;
    private Vector2 velocity;

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
    #endregion

    /// <summary>
    /// Draws debug info to the screen
    /// </summary>
    private void OnGUI() {
        if (showDebug) {
            GUI.Label(new Rect(5, 5, 300, 150), $"activeMoveSpeed: {activeMoveSpeed}");
            GUI.Label(new Rect(5, 20, 300, 150), $"dashCharges: {currentDashCharges}");
            GUI.Label(new Rect(5, 35, 300, 150), $"playerState: {playerState}");
        }
    }

    /// <summary>
    /// Runs when the object is enabled for the first time
    /// </summary>
    void Start() { 
        animator = GetComponent<Animator>();

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
                break;
        }
    }

    /// <summary>
    /// Runs every physics frame (default is 50 times a second)
    /// </summary>
    void FixedUpdate() {
        transform.Translate(velocity * activeMoveSpeed * Time.fixedDeltaTime);
    }

    #region Input
    /// <summary>
    /// Interprets the player's input for movement
    /// </summary>
    /// <param name="context"></param>
    public void Move(InputAction.CallbackContext context) {
        velocity = context.ReadValue<Vector2>().normalized;
    }

    #region
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
            canDash = false;
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
        playerState = PlayerState.Idle;
        activeMoveSpeed = walkSpeed;

        yield return new WaitForSeconds(dashRechargeTime);
        currentDashCharges++;
    }

    /// <summary>
    /// Creates the afterimage clones of the player while dashing
    /// Every other physics frame a clone is created
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateAfterImages() {
        bool shouldSpawnAfterImage = false;
        while (playerState == PlayerState.Dashing) {
            if (shouldSpawnAfterImage = !shouldSpawnAfterImage) {
                GameObject echoInstance = Instantiate(afterImage, transform.position, Quaternion.identity);
                echoInstance.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
            }
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }
    #endregion
    #endregion
}