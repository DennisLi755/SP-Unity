using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterHouse : Trigger {
    protected override void OnTrigger() {
        if (GameManager.Instance.GetProgressionFlag("Met Ghost Dog")) {
            DialogueManager.Instance.StartDialogue("Enter_House");
        }
        else if (GameManager.Instance.GetProgressionFlag("Second Outside")) {
            UIManager.Instance.ActivateInteractText(new List<string> { "You should probably do what GUIDE says." });
        }
    }
}
