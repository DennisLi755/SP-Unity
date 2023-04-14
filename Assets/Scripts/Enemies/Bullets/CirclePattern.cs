using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CirclePattern : BulletPattern {
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    private int bulletCount = 8;
    [SerializeField]
    private float startingAngle = 0.0f;
    [SerializeField]
    private float decayTime = 0.0f;

    private new void Start() {
        float angleStep = 360 / bulletCount;
        for (int i = 0; i < bulletCount; i++) {
            Bullet newBullet = Instantiate(bullet, transform).GetComponent<Bullet>();
            newBullet.Angle = angleStep * i + startingAngle;
            if (decayTime != 0.0f) {
                newBullet.DecayTime = decayTime;
            }
            if (speed != 0.0f) {
                newBullet.Speed = speed;
            }
        }
    }

    public override void SetOverrideSettings(string settings) {
        string[] splitSettings = settings.Split(',');
        foreach (string s in splitSettings) {
            int equal = s.IndexOf('=');
            if (equal == -1) {
                Debug.LogWarning($"{s} does not contain a \'=\', therefore the pattern setting is unable to be set");
                continue;
            }
            string setting = s.Substring(0, equal).Trim();
            float value = Single.Parse(s.Substring(equal + 1).Trim());
            switch (setting) {
                case "startingAngle": startingAngle = value; break;
                case "decay": decayTime = value; break;
                case "speed": speed = value; break;
                case "bulletCount": bulletCount = (int)value; break;
                default: Debug.LogWarning($"Setting {s} does not exist for ${this}"); break;
            }
        }
    }
}
