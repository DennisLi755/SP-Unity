using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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

    void Start() {
        dashSpeed = 30f;
        echoSpawns = 0.01f;
        activeMoveSpeed = moveSpeed;
        isDashing = false;
        isAttacking = false;
    }


    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && isAttacking)
            isAttacking = false;

        Dash();
        Attack();

        if (movement != Vector2.zero) {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }

        animator.SetFloat("Speed", movement.sqrMagnitude);

        animator.SetBool("IsAttacking", isAttacking);
    }

    void FixedUpdate()
    {
        if (!isAttacking)
            rb.MovePosition(rb.position + movement * activeMoveSpeed * Time.fixedDeltaTime);
        if (isDashing) {
            DashEffect();
        }
    }

    void Dash() {
        if (Input.GetKeyDown(KeyCode.X) && movement.sqrMagnitude > 0) {
            if (dashCoolCounter <= 0 && dashCounter <= 0) {
                isDashing = true;
                isAttacking = false;
                activeMoveSpeed = dashSpeed;
                dashCounter = dashLength;
            }
        }

        if (dashCounter > 0) {
            dashCounter -= Time.deltaTime;
            if (dashCounter <= 0) { 
                isDashing = false;
                activeMoveSpeed = moveSpeed;
                dashCoolCounter = dashCooldown;
            }
        }

        if (dashCoolCounter > 0) {
            dashCoolCounter -= Time.deltaTime;
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

    void Attack() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            isAttacking = true;
            isDashing = false;
        }
    }
}
