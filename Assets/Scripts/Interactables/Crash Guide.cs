using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CrashGuide : InteractableObject {
    private Animator anim;

    new void Start() {
        base.Start();
        anim = GetComponent<Animator>();
    }

    public override void OnInteract() {
        if (GameManager.Instance.GetProgressionFlag("Second Outside")) {
            DialogueManager.Instance.StartDialogue("Gravestone_Interact");
        } else if (GameManager.Instance.GetProgressionFlag("Leave House First Time")) {
            DialogueManager.Instance.StartDialogue("Guide_Interact_After_Boss");
        } else if (GameManager.Instance.GetProgressionFlag("Not Ready")) {
            DialogueManager.Instance.StartDialogue("Are_You_Ready");
        } else {
            DialogueManager.Instance.StartDialogue("Meet_Guide");
            GameManager.Instance.SavePlayerData("Save Toilet - Guide Fight");
        }
    }
}