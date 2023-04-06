using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneInteractable : InteractableObject
{
    [SerializeField]
    private string cutsceneName;

    public override void OnInteract()
    {
        if (PlayerInfo.Instance.PlayerControl.FacingDirection == direction)
        {
            DialogueManager.Instance.StartDialogue(cutsceneName);
        }
    }
}
