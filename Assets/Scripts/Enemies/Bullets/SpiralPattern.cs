using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralPattern : BulletPattern {

    [SerializeField]
    [Range(0, 360)]
    float startingAngle = 0.0f;
    [SerializeField]
    [Range(1, 359)]
    private float angleStep;
    [SerializeField]
    private float delay = 0.05f;
    [SerializeField]
    private float fullRotations = 1;
    [SerializeField]
    GameObject bullet;

    new void Start() {
        StartCoroutine(Shoot());
    }

    new void Update() { }

    IEnumerator Shoot() {
        float currAngle = startingAngle;
        while (currAngle / 360.0f < fullRotations) {
            GameObject b = Instantiate(bullet,transform);
            Bullet bScript;
            if (b.TryGetComponent<Bullet>(out bScript)) {
                bScript.Angle = currAngle;
                if (overrideSpeed) {
                    bScript.Speed = speed;
                }
            }
            yield return new WaitForSeconds(delay);
            currAngle += angleStep;
        }
    }
}