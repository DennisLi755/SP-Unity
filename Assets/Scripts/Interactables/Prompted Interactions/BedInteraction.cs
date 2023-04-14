using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedInteraction : PromptedInteraction
{
    public override void OnInteract() {
        if (!GameManager.Instance.GetProgressionFlag("Saw Void") || GameManager.Instance.GetProgressionFlag("Second Awake")) {
            UIManager.Instance.ActivateInteractText(new List<string>{"Your bed.", "You resist the urge to go back to bed immediately..."});
        } else {
            base.OnInteract();
        }
    }

    public override void OnYes()
    {
        UIManager.Instance.ResetPromptedInteraction();
        DialogueManager.Instance.StartDialogue("Back_To_Bed");
    }
}
