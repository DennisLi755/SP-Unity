using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField]
    private LayerMask playerLayer;
    private new BoxCollider2D collider;
    private new SpriteRenderer sr;
    void Start() {
        collider = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(WaitForDespawn(5.0f));
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

    IEnumerator WaitForDespawn(float time) {
        yield return new WaitForSeconds(time);

        const int flashCount = 15;
        bool fullOpacity = true;
        for (int i = 0; i < flashCount; i++, fullOpacity = !fullOpacity) {
            Color newColor = sr.color;
            newColor.a = fullOpacity ? 1.0f : 0.5f;
            sr.color = newColor;
            yield return new WaitForSeconds(time / flashCount);
        }
        gameObject.SetActive(false);
    }
}
