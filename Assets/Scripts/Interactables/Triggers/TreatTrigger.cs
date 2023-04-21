using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreatTrigger : CutsceneTrigger
{
    protected override void OnTrigger()
    {
        if (!GameManager.Instance.GetProgressionFlag("First Treat") ||
        !GameManager.Instance.GetProgressionFlag("Second Treat") ||
        !GameManager.Instance.GetProgressionFlag("Third Treat")) {
            base.OnTrigger();
        }
    }
}
