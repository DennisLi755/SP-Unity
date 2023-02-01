using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxInteraction : InteractableObject {
    [SerializeField]
    private List<string> message;
    private FacingDirection direction;
    private bool isChild = false;

    void Start() {
        base.Start();
        if (isChild) {
            message = transform.parent.gameObject.GetComponent<TextboxInteraction>().message;
        }
    }

    public override void OnInteract() {
        if (PlayerInfo.Instance.PlayerControl.FacingDirection == direction) {
            //Debug.Log("Textbox interaction triggered");
            UIManager.Instance.ActivateInteractText(message);
        }
    }
}