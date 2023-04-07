using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeaveHouse : EnterText
{
    public override void OnInteract() {
        if (GameManager.Instance.GetProgressionFlag("Leave House First Time")) {
            DialogueManager.Instance.StartDialogue("Second_Outside");
        } else if (GameManager.Instance.GetProgressionFlag("Saw Void")) {
            UIManager.Instance.ActivateInteractText(new List<string>{"You refuse to open the door."});
        } else if (!PlayerInfo.Instance.AttackUnlocked) {
            UIManager.Instance.ActivateInteractText(new List<string>{"Weirdly enough, the door is locked and needs a key to open it from the inside."});
        } else {
            DialogueManager.Instance.StartDialogue("First_Outside");
        }
    }
}
