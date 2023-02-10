using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class GuideInteract : InteractableObject {
    [SerializeField]
    private YarnProject project;
    [SerializeField]
    private string startNode;
    public override void OnInteract() {
        DialogueManager.Instance.StartDialogue(project, startNode);

        base.OnInteract();
    }
}