using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralPattern : BulletPattern {

    [SerializeField]
    [Range(0, 360)]
    private float startingAngle = 0.0f;
    [SerializeField]
    [Range(1, 359)]
    private float angleStep;
    [SerializeField]
    private float delay = 0.05f;
    [SerializeField]
    private float fullRotations = 1;
    [SerializeField]
    private float decayTime = 0;
    [SerializeField]
    GameObject bullet;

    new void Start() {
        StartCoroutine(Shoot());
    }

    new void Update() { }

    IEnumerator Shoot() {
        float currAngle = startingAngle;
        while ((currAngle - startingAngle) / 360.0f < fullRotations) {
            GameObject b = Instantiate(bullet, transform);
            Bullet bScript;
            if (b.TryGetComponent<Bullet>(out bScript)) {
                bScript.Angle = currAngle;
                if (overrideSpeed) {
                    bScript.Speed = speed;
                }
                if (decayTime != 0) {
                    bScript.DecayTime = decayTime;
                }
            }
            yield return new WaitForSeconds(delay);
            currAngle += angleStep;
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
            float value = float.Parse(s.Substring(equal+1).Trim());
            switch (setting) {
                case "startingAngle": startingAngle = value; break;
                case "angleStep": angleStep = value; break;
                case "delay": delay = value; break;
                case "fullRotations": fullRotations = value; break;
                case "decay": decayTime = value; break;
                default: Debug.LogWarning($"Setting {s} does not exist for ${this}"); break;
            }
        }
    }
}