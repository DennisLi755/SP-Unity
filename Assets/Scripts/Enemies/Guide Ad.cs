using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideAd : StaticEnemy
{
    private bool hasArrived = false;
    public bool HasArrived {get => hasArrived; set => hasArrived = value; }
    void Start() {
        base.Start();
        bodyCollider.gameObject.SetActive(false);
        StartCoroutine(WaitForArrive());
    }

    // Update is called once per frame
    void Update()
    {
        if (hasArrived) {
            base.Update();
        }
    }

    public IEnumerator WaitForArrive() {
        while (!hasArrived) {
            GameObject echoInstance = Instantiate(afterImage, transform.position, Quaternion.identity);
            echoInstance.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime * 3);
        }

        bodyCollider.gameObject.SetActive(true);
    }
}
