using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneInteractable : InteractableObject {
    [SerializeField]
    private string cutsceneName;
    [SerializeField]
    private string checkForProgressionFlag;

    public override void OnInteract() {
        if (ValidatePlayerDirection() && 
        (checkForProgressionFlag != null && !GameManager.Instance.GetProgressionFlag(checkForProgressionFlag) || checkForProgressionFlag == "")) {
            DialogueManager.Instance.StartDialogue(cutsceneName);
        }
    }
}
