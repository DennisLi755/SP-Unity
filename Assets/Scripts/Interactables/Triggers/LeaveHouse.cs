using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeaveHouse : Trigger {
    private Room containingRoom;
    private readonly List<string> beforeSwordMessages = new List<string>() {
        "Weirdly enough, the door is locked and needs a key to open it from the inside.",
        "You last remember seeing the key on the coffee table."
    };

    new void Start() {
        base.Start();
        containingRoom = transform.parent.parent.GetComponent<Room>();
    }
    protected override void OnTrigger() {
        if (GameManager.Instance.GetProgressionFlag("Leave House First Time")) {
            DialogueManager.Instance.StartDialogue("Second_Outside");
        }
        else if (GameManager.Instance.GetProgressionFlag("Saw Void")) {
            UIManager.Instance.ActivateInteractText(new List<string> { "You refuse to open the door." });
        }
        else if (!PlayerInfo.Instance.AttackUnlocked) {
            UIManager.Instance.ActivateInteractText(beforeSwordMessages);
        }
        else {
            containingRoom.onExit?.Invoke();
            DialogueManager.Instance.StartDialogue("First_Outside");
        }
    }
}
