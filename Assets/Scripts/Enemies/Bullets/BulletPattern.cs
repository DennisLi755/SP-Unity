using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPattern : MonoBehaviour {

    protected Bullet[] bullets;
    [SerializeField]
    protected bool overrideSpeed = false;
    [SerializeField]
    protected float speed;
    protected GameObject player;
    [SerializeField]
    protected BulletType bulletType;

    protected void Start() {
        bullets = transform.GetComponentsInChildren<Bullet>();
        player = PlayerInfo.Instance.gameObject;
        if (overrideSpeed) {
            for (int i = 0; i < bullets.Length; i++) {
                bullets[i].Speed = speed;
            }
            Array.Clear(bullets, 0, bullets.Length);
        }
        /*
        for (int i = 0; i < bullets.Length; i++) {
            bullets[i].BulletType = bulletType;
        }*/
    }

    protected void Update() {
        if (transform.childCount == 0) {
            Destroy(gameObject);
        }
    }

    public void SetSpeedOverride(float speed) {
        overrideSpeed = true;
        this.speed = speed;
    }
}