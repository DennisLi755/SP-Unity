using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideAd : StaticEnemy
{
    private bool hasArrived = false;
    public bool HasArrived {get => hasArrived; set => hasArrived = value; }
    // Update is called once per frame
    void Update()
    {
        if (hasArrived) {
            base.Update();
        }
    }
}
