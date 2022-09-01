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

    public float moveSpeed = 7f;

    public Rigidbody2D rb;
    private Animator animator;

    Vector2 movement;

    private float activeMoveSpeed;
    public float dashSpeed;
    private float stamina;
    private float staminaCounter;

    private float dashLength = 0.15f;
    private float dashCooldown = 0.2f;

    private float dashCounter;
    private float dashCoolCounter;

    public GameObject echo;
    private float echoSpawns;
    private bool isDashing;

    private State playerState;

    void Start() {
        dashSpeed = 30f;
        echoSpawns = 0.01f;
        activeMoveSpeed = moveSpeed;
        playerState = State.Idle;
        animator = GetComponent<Animator>();
        stamina = MAX_STAMINA;
        staminaCounter = 0f;
    }


    // Update is called once per frame
    void Update()
    {
        GetInput();
        animator.SetBool("IsAttacking", (playerState == State.Attack)? true : false);
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
                Move();
                if (dashCounter > 0) {
                    activeMoveSpeed = dashSpeed;
                    dashCounter -= Time.deltaTime;
                    if (dashCounter <= 0) { 
                        playerState = State.Idle;
                        activeMoveSpeed = moveSpeed;
                    }
                }
                break;
            case State.Attack:
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 &&
                    animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
                    playerState = State.Idle;
                }
                break;
        }
        Debug.Log(playerState);
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
            if (dashCoolCounter <= 0 && dashCounter <= 0 && playerState != State.Dash) { 
                playerState = State.Dash;
                dashCounter = dashLength;
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
