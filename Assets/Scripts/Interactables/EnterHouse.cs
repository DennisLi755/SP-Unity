using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterHouse : EnterText
{
    public override void OnInteract() {
        if (GameManager.Instance.GetProgressionFlag("Met Ghost Dog")) {
            DialogueManager.Instance.StartDialogue("Enter_House");
        } else {
            UIManager.Instance.ActivateInteractText(new List<string>{"You should probably do what GUIDE says."});
        }
    }
}
