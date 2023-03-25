using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class GuideInteract : InteractableObject {
    public override void OnInteract() {
        DialogueManager.Instance.StartDialogue("Guide_Interact");
    }
}