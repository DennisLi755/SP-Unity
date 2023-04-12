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

    public virtual void SetOverrideSettings(string settings) { 
        string[] splitSettings = settings.Split(',');
        foreach (string s in splitSettings) {
            int equal = s.IndexOf('=');
            if (equal == -1) {
                Debug.LogWarning($"{s} does not contain a \'=\', therefore the pattern setting is unable to be set");
                continue;
            }
            string setting = s.Substring(0, equal).Trim();
            float value = Single.Parse(s.Substring(equal+1).Trim());
            switch (setting) {
                case "speed": speed = value; break;
                default: Debug.LogWarning($"Setting {s} does not exist for ${this}"); break;
            }
        }
        Debug.LogError($"The setting override for {this.name} has not been setup"); 
    }
}