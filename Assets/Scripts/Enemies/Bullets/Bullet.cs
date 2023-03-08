using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public enum BulletType {
    Normal,
    Deflectable
}

public class Bullet : MonoBehaviour {

    [Range(0, 359)]
    [SerializeField]
    private float angle;
    public float Angle {get => angle;}
    [SerializeField]
    private float speed;
    public float Speed { get => speed; set { speed = value; } }
    private Vector2 direction;
    public Vector2 Direction {get => direction; set => direction = value; }
    [SerializeField]
    private LayerMask playerLayer;
    private new CircleCollider2D collider;
    private new SpriteRenderer renderer;
    private bool hasRendered = false;
    private float timeOffScreen = 0.0f;
    private float maxTimeOffScreen = 5.0f;
    [SerializeField]
    private BulletType bulletType;
    public BulletType BulletType {get => bulletType; set => bulletType = value; }

    void Start() {
        BulletHolder.Instance.AddBullet();

        collider = GetComponent<CircleCollider2D>();
        renderer = GetComponent<SpriteRenderer>();

        direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)).normalized;
        StartCoroutine(WaitForRender());

        IEnumerator WaitForRender() {
            yield return new WaitForSeconds(0.5f);
            hasRendered = true;
        }
        if (bulletType == BulletType.Deflectable) {
            renderer.color = Color.black;
        }
    }

    void Update() {
        /*RaycastHit2D hit = Physics2D.CircleCast(collider.bounds.center, collider.radius, Vector2.zero, 0.0f, playerLayer);
        if (hit) {
            PlayerInfo.Instance.Hurt(1);
        }*/

        if (hasRendered && !renderer.isVisible) {
            timeOffScreen += Time.deltaTime;
            if (timeOffScreen > maxTimeOffScreen) {
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate() {
        transform.Translate(speed * Time.fixedDeltaTime * direction);
    }

    private void OnDestroy() {
        BulletHolder.Instance.RemoveBullet();
    }
}