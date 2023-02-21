using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField]
    private LayerMask playerLayer;
    private new BoxCollider2D collider;
    void Start() {
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0.0f, Vector2.zero, 0.0f, playerLayer);
        if (hit) {
            PlayerInfo.Instance.Heal(1);
            gameObject.SetActive(false);
        }
    }
}
