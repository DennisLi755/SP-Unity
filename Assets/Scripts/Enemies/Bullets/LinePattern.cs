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
    private float decayTime = 0.0f;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private bool tracking;
    // Start is called before the first frame update
    new void Start()
    {
        if (tracking) {
            GameObject player = PlayerInfo.Instance.gameObject;
            angle = Mathf.Rad2Deg * Mathf.Atan2(player.transform.position.y - transform.position.y, 
                player.transform.position.x  - transform.position.x);
            Debug.Log(angle);
        }
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot() {
        while (length >= 0.0f) {
            GameObject b = Instantiate(bullet, transform);
            Bullet bScript;
            if (b.TryGetComponent<Bullet>(out bScript)) {
                bScript.Angle = angle;
                if (overrideSpeed) {
                    bScript.Speed = speed;
                }
                if (decayTime != 0) {
                    bScript.DecayTime = decayTime;
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
                case "decay": decayTime = value; break;
                case "speed": speed = value; break;
                default: Debug.LogWarning($"Setting {s} does not exist for ${this}"); break;
            }
        }
    }
}
