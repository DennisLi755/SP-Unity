using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : Trigger {
    protected override void OnTrigger() {
        PlayerInfo.Instance.EnableTutorialText(
            "Press X to dodge through enemy bullets",
            "Press B to dodge through enemy bullets"
            );
        PlayerInfo.Instance.Tutorials.Add("dashing", false);
        base.OnTrigger();
    }
}
