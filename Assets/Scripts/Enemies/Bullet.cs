using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour {

    [SerializeField]
    private Vector2 velocity;
    [SerializeField]
    private LayerMask playerLayer;
    private new CircleCollider2D collider;
    private new SpriteRenderer renderer;
    
    void Start() {
        collider = GetComponent<CircleCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        RaycastHit2D hit = Physics2D.CircleCast(collider.bounds.center, collider.radius, Vector2.zero, 0.0f, playerLayer);
        if (hit) {
            PlayerInfo.Instance.Hurt(1);
        }
    }

    private void FixedUpdate() {
        transform.Translate(velocity * Time.fixedDeltaTime);
    }
}
