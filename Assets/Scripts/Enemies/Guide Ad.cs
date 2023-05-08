using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideAd : RailEnemy {

    [SerializeField]
    protected GameObject afterImage;

    private void Awake() {
        GetComponent<BoxCollider2D>().enabled = false;
        gameObject.layer = 0;
    }

    new void Start() {
        base.Start();
        StartCoroutine(WaitForArrive());
        canContinueAttack = false;
        canAttack = true;
    }

    /// <summary>
    /// Creates Echo/After Images of the ad until it reaches its target, also enables its attacking and hitbox when it reaches its target
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitForArrive() {
        while (transform.position != Target) {
            GameObject echoInstance = Instantiate(afterImage, transform.position, Quaternion.identity);
            echoInstance.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime * 3);
        }

        GetComponent<BoxCollider2D>().enabled = true;
        canContinueAttack = true;
        gameObject.layer = 9;
    }
}