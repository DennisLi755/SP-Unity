using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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
    void Start() {
        dashSpeed = 30f;
        echoSpawns = 0.01f;
        activeMoveSpeed = moveSpeed;
        isDashing = false;
    }


    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.X)) {
            if (dashCoolCounter <= 0 && dashCounter <= 0) {
                isDashing = true;
                Instantiate(echo, transform.position, Quaternion.identity);
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

        if (movement != Vector2.zero) {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }

        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * activeMoveSpeed * Time.fixedDeltaTime);
        if (isDashing) {
            if (echoSpawns <= 0) {
                GameObject echoInstance = Instantiate(echo, transform.position, Quaternion.identity);
                echoInstance.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
                Destroy(echoInstance, 1f);
                echoSpawns = 0.01f;
            } else {
                echoSpawns -= Time.deltaTime*2;
            }
        }
    }
}
