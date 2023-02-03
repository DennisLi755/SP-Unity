using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPattern : MonoBehaviour {

    Bullet[] bullets;
    [SerializeField]
    private bool overrideSpeed;
    [SerializeField]
    private float speed;

    void Start() {
        if (overrideSpeed) {
            bullets = transform.GetComponentsInChildren<Bullet>();
            for (int i = 0; i < bullets.Length; i++) {
                bullets[i].Speed = speed;
            }
            Array.Clear(bullets, 0, bullets.Length);
        }
    }

    void Update() {
        if (transform.childCount == 0) {
            Destroy(gameObject);
        }
    }

    public void SetSpeedOverride(float speed) {
        overrideSpeed = true;
        this.speed = speed;
    }
}