using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum State {
        Idle,
        Attack,
        Dash
    }
    const float MAX_STAMINA = 10f;

    private float moveSpeed = 5f;

    public Rigidbody2D rb;
    private Animator animator;

    Vector2 movement;

    private float activeMoveSpeed;
    public float dashSpeed;
    private float stamina;
    private float staminaCounter;

    private float dashLength = 0.15f;

    private bool isDashing;
    private bool isAttacking;

    public GameObject echo;
    private float echoSpawns;

    private State playerState;
    int totalAttackFrames;
    public int damageFrame;
    public AnimationClip attackAnimation;

    void Start() {
        dashSpeed = 20f;
        echoSpawns = 0.01f;
        activeMoveSpeed = moveSpeed;
        playerState = State.Idle;
        animator = GetComponent<Animator>();
        stamina = MAX_STAMINA;
        staminaCounter = 0f;
        isDashing = false;
        totalAttackFrames = (int)(attackAnimation.length * attackAnimation.frameRate);
        isAttacking = false;
    }


    // Update is called once per frame
    void Update()
    {
        GetInput();
        animator.SetBool("IsAttacking", playerState == State.Attack);
        switch (playerState) {
            case State.Idle:
                Move();
                if (movement != Vector2.zero) {
                    animator.SetFloat("Horizontal", movement.x);
                    animator.SetFloat("Vertical", movement.y);
                }
                animator.SetFloat("Speed", movement.sqrMagnitude);
                break;
            case State.Dash:
                if (isAttacking)
                    isAttacking = false;
                Move();
                if (!isDashing) {
                    activeMoveSpeed = dashSpeed;
                    StartCoroutine(DisableDash());
                    isDashing = true;
                }
                break;
            case State.Attack:
                if (!isAttacking) {
                    DisableAttack();
                    isAttacking = true;
                }
                break;
        }
        Debug.Log(playerState);
    }

    IEnumerator DisableDash() {
        yield return new WaitForSeconds(0.12f);
        playerState = State.Idle;
        activeMoveSpeed = moveSpeed;
        isDashing = false;
    }

    IEnumerator DisableAttack() {
        //yield return new WaitForSeconds((float)damageFrame / totalAttackFrames * attackAnimation.length);
        //attack code

        yield return new WaitForSeconds((1.0f - (float)damageFrame / totalAttackFrames) * attackAnimation.length);
        playerState = State.Idle;
        isAttacking = false;
    }

    void FixedUpdate()
    {
        if (playerState != State.Attack)
            rb.MovePosition(rb.position + movement * activeMoveSpeed * Time.fixedDeltaTime);
        if (playerState == State.Dash) {
            DashEffect();
        }

        if (stamina < MAX_STAMINA) {
            staminaCounter++;
            if (staminaCounter >= 100) {
                stamina += 0.5f;
            }
        } else {
            staminaCounter = 0;
        }
    }

    void Move() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void GetInput() {
        if (Input.GetKeyDown(KeyCode.X) && stamina > 0) {
            if (!isDashing && playerState != State.Dash) { 
                playerState = State.Dash;
                stamina -= 5;
            }
        } else if (Input.GetKeyDown(KeyCode.Z) && playerState != State.Attack) {
            playerState = State.Attack;
            activeMoveSpeed = moveSpeed;
        }
    }

    void DashEffect() {
        if (echoSpawns <= 0) {
            GameObject echoInstance = Instantiate(echo, transform.position, Quaternion.identity);
            echoInstance.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
            echoSpawns = 0.01f;
        } else {
            echoSpawns -= Time.deltaTime*2;
        }
    }
}
