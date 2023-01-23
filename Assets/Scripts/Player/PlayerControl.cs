using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public struct RayCastOrigins {
    public Vector2 topLeft, topRight, bottomLeft, bottomRight;
}

public struct CollisionDirections {
    public bool up, down, right, left;
    public void Reset() {
        up = down = right = left = false;
    }
}

public struct FacingDirection {
    bool up, down, left, right;
}

public enum PlayerState {
    Idle,
    Attack,
    Dash
}

public class PlayerControl : MonoBehaviour {

    private Animator animator;

    #region Movement
    //movement
    [SerializeField]
    private const float walkSpeed = 5f;
    private Vector2 velocity;

    //dashing
    private int maxDashCharges = 2;
    private int currentDashCharges = 0;
    private const float dashSpeed = 20f;

    //active move speed is changed between the value of walkSpeed and dashSpeed when the player starts or ends a dash
    //by default is is equal to walkSpeed
    private float activeMoveSpeed = walkSpeed;
    #endregion







    private bool isDashing;
    private bool isAttacking;

    public GameObject echo;
    private float echoSpawns;

    private PlayerState playerState = PlayerState.Idle;
    int totalAttackFrames;
    public int damageFrame;
    public AnimationClip attackAnimation;

    private void OnGUI() {
        GUI.Label(
            new Rect(5, 50, 300, 150),
            $"activeMoveSpeed: {activeMoveSpeed}");
    }

    void Start() { 
        animator = GetComponent<Animator>();

        echoSpawns = 0.01f;
        isDashing = false;
        totalAttackFrames = (int)(attackAnimation.length * attackAnimation.frameRate);
        isAttacking = false;
    }

    // Update is called once per frame
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
            case PlayerState.Dash:
                if (isAttacking)
                    isAttacking = false;
                if (!isDashing) {
                    activeMoveSpeed = dashSpeed;
                    StartCoroutine(DisableDash());
                    isDashing = true;
                }
                break;
            case PlayerState.Attack:
                if (!isAttacking) {
                    StartCoroutine(DisableAttack());
                    isAttacking = true;
                }
                break;
        }
        //Debug.Log(playerState);
    }

    void FixedUpdate() {
        transform.Translate(velocity * activeMoveSpeed * Time.fixedDeltaTime);
        if (playerState == PlayerState.Dash) {
            DashEffect();
        }

        DashRecharge();
    }

    IEnumerator DisableDash() {
        yield return new WaitForSeconds(0.12f);
        playerState = PlayerState.Idle;
        activeMoveSpeed = walkSpeed;
        isDashing = false;
    }

    IEnumerator DisableAttack() {
        yield return new WaitForSeconds((float)damageFrame / totalAttackFrames * attackAnimation.length);
        //attack code

        yield return new WaitForSeconds((1.0f - (float)damageFrame / totalAttackFrames) * attackAnimation.length);
        playerState = PlayerState.Idle;
        isAttacking = false;
    }

    void DashRecharge() {
        //after 2 seconds give back dash charge
    }

    public void Move(InputAction.CallbackContext context) {
        velocity = context.ReadValue<Vector2>().normalized;
    }
    
    public void Attack(InputAction.CallbackContext context) {
        if (context.performed && playerState != PlayerState.Attack) {
            playerState = PlayerState.Attack;
            activeMoveSpeed = walkSpeed;
        }
    }

    public void Dash(InputAction.CallbackContext context) {
        if (context.performed && currentDashCharges > 0) {
            if (!isDashing && playerState != PlayerState.Dash) {
                playerState = PlayerState.Dash;
                currentDashCharges--;
            }
        }
    }

    void DashEffect() {
        if (echoSpawns <= 0) {
            GameObject echoInstance = Instantiate(echo, transform.position, Quaternion.identity);
            echoInstance.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
            echoSpawns = 0.01f;
        }
        else {
            echoSpawns -= Time.deltaTime * 2;
        }
    }
}
