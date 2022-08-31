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

    public float moveSpeed = 7f;

    public Rigidbody2D rb;
    public Animator animator;

    Vector2 movement;

    private float activeMoveSpeed;
    public float dashSpeed;

    private float dashLength = 0.15f;
    private float dashCooldown = 0.2f;

    private float dashCounter;
    private float dashCoolCounter;

    public GameObject echo;
    private float echoSpawns;
    private bool isDashing;

    private bool isAttacking;

    private State playerState;

    void Start() {
        dashSpeed = 30f;
        echoSpawns = 0.01f;
        activeMoveSpeed = moveSpeed;
        isDashing = false;
        isAttacking = false;
        playerState = State.Idle;
    }


    // Update is called once per frame
    void Update()
    {
        GetInput();
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
                        dashCoolCounter = dashCooldown;
                    }
                }
                break;
            case State.Attack:
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    playerState = State.Idle;
                    isAttacking = false;
                break;
        }
        if (dashCoolCounter > 0) {
            dashCoolCounter -= Time.deltaTime;
        }
        animator.SetBool("IsAttacking", isAttacking);
        Debug.Log(playerState);
    }

    void FixedUpdate()
    {
        if (playerState != State.Attack)
            rb.MovePosition(rb.position + movement * activeMoveSpeed * Time.fixedDeltaTime);
        if (playerState == State.Dash) {
            DashEffect();
        }
    }

    void Move() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void GetInput() {
        if (Input.GetKeyDown(KeyCode.X)) {
            if (dashCoolCounter <= 0 && dashCounter <= 0 && movement.sqrMagnitude > 0 && playerState != State.Dash) { 
                playerState = State.Dash;
                dashCounter = dashLength;
            }
        } else if (Input.GetKeyDown(KeyCode.Z) && playerState != State.Attack) {
            playerState = State.Attack;
            isAttacking = true;
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
