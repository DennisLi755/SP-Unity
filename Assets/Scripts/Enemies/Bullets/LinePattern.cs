using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePattern : BulletPattern
{
    [SerializeField]
    private float angle = 0.0f;
    [SerializeField]
    private float length = 5.0f;
    [SerializeField]
    private float delay = 0.5f;
    [SerializeField]
    private GameObject bullet;
    // Start is called before the first frame update
    new void Start()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot() {
        if (length >= 0.0f) {
            GameObject b = Instantiate(bullet, transform);
            Bullet bScript;
            if (b.TryGetComponent<Bullet>(out bScript)) {
                bScript.Angle = angle;
                if (overrideSpeed) {
                    bScript.Speed = speed;
                }
            }
            length--;
            yield return new WaitForSeconds(delay);
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
            float value = Single.Parse(s.Substring(equal+1).Trim());
            switch (setting) {
                case "angle": angle = value; break;
                case "delay": delay = value; break;
                case "length": length = value; break;
                default: Debug.LogWarning($"Setting {s} does not exist for ${this}"); break;
            }
        }
    }
}
