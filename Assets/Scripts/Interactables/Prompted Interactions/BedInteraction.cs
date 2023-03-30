using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedInteraction : PromptedInteraction
{
    void OnInteract() {
        if (!GameManager.Instance.GetProgressionFlag("Saw Void")) {
            UIManager.Instance.ActivateInteractText(new List<string>{"Your bed.", "You resist the urge to go back to bed immediately..."});
        } else {
            base.OnInteract();
        }
    }

    public override void OnYes()
    {
        DialogueManager.Instance.StartDialogue("Back_To_Bed");
    }
}