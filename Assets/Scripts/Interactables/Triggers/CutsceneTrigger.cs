using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CutsceneTrigger : Trigger
{
    [SerializeField]
    private string cutsceneName;
    [SerializeField]
    private bool happensOnce;

    protected override void OnTrigger() {
        DialogueManager.Instance.StartDialogue(cutsceneName);
        if (happensOnce) {
            base.OnTrigger();
        }
    }
}
