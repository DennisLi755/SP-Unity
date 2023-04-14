using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeaveHouse : EnterText
{
    private Room containingRoom;

    new void Start() {
        containingRoom = transform.parent.parent.GetComponent<Room>();
    }
    public override void OnInteract() {
        if (GameManager.Instance.GetProgressionFlag("Leave House First Time")) {
            DialogueManager.Instance.StartDialogue("Second_Outside");
        } else if (GameManager.Instance.GetProgressionFlag("Saw Void")) {
            UIManager.Instance.ActivateInteractText(new List<string>{"You refuse to open the door."});
        } else if (!PlayerInfo.Instance.AttackUnlocked) {
            UIManager.Instance.ActivateInteractText(new List<string>{"Weirdly enough, the door is locked and needs a key to open it from the inside."});
        } else {
            containingRoom.onExit?.Invoke();
            DialogueManager.Instance.StartDialogue("First_Outside");
        }
    }
}
