using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxInteraction : InteractableObject {
    [SerializeField]
    private List<string> message;
    [SerializeField]
    private bool isChild = false;

    public List<string> Message { get => message; set => message = value; }

    new void Start() {
        base.Start();
        if (isChild) {
            message = transform.parent.gameObject.GetComponent<TextboxInteraction>().message;
        }
    }

    public override void OnInteract() {
        if (ValidatePlayerDirection()) {
            UIManager.Instance.ActivateInteractText(message);
        }
    }
}