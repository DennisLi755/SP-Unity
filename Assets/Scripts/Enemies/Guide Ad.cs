using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideAd : RailEnemy {

    [SerializeField]
    protected GameObject afterImage;

    void Start() {
        base.Start();
        StartCoroutine(WaitForArrive());
        canContinueAttack = false;
        canAttack = true;
    }

    public IEnumerator WaitForArrive() {
        while (transform.position != Target) {
            GameObject echoInstance = Instantiate(afterImage, transform.position, Quaternion.identity);
            echoInstance.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime * 3);
        }

        GetComponent<BoxCollider2D>().enabled = true;
        canContinueAttack = true;
    }
}