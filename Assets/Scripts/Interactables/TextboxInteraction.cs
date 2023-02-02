using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxInteraction : InteractableObject {
    [SerializeField]
    private List<string> message;
    [SerializeField]
    private FacingDirection direction;
    [SerializeField]
    private bool isChild = false;

    void Start() {
        base.Start();
        if (isChild) {
            message = transform.parent.gameObject.GetComponent<TextboxInteraction>().message;
        }
    }

    public override void OnInteract() {
        if (PlayerInfo.Instance.PlayerControl.FacingDirection == direction) {
            UIManager.Instance.ActivateInteractText(message);
        }
    }
}