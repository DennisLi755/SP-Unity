using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeaveHouse : EnterText
{
    public override void OnInteract() {
        if (!PlayerInfo.Instance.AttackUnlocked) {
            UIManager.Instance.ActivateInteractText(new List<string>{"Weirdly enough, the door is locked and needs a key to open it from the inside."});
        } else {
            DialogueManager.Instance.StartDialogue("First_Outside");
        }
    }

    void EndInteraction() {
        UIManager.Instance.FadeFromBlack();
        UIManager.Instance.endInteractionEvent.RemoveAllListeners();
        DialogueManager.Instance.StartDialogue("Enter_House");
    }
}
