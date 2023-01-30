using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField]
    private Vector2 velocity;
    [SerializeField]
    private LayerMask playerLayer;
    private new CircleCollider2D collider;
    
    void Start() {
        collider = GetComponent<CircleCollider2D>();
    }

    void Update() {
        //transform.Rotate(0.0f, 0.0f, 0.5f);
        RaycastHit2D hit = Physics2D.CircleCast(collider.bounds.center, collider.radius, Vector2.zero, 0.0f, playerLayer);
        if (hit) {
            PlayerInfo.Instance.Hurt(1);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        transform.Translate(velocity * Time.fixedDeltaTime);
    }
}
