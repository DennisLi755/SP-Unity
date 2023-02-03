using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SwordInteraction : PromptedInteraction
{
    [SerializeField]
    private Tilemap tileMap;
    [SerializeField]
    private Vector3Int position;

    public override void OnInteract()
    {
        if (PlayerInfo.Instance.PlayerControl.UnlockAttack == false) {
            base.OnInteract();
        } else {
            UIManager.Instance.ActivateInteractText(new List<string>{"Just a regular coffee table."});
        }
    }

    public override void OnYes()
    {
        PlayerInfo.Instance.PlayerControl.UnlockAttack = true;
        //code to remove sword sprite from world
        tileMap.SetTile(position, null);
        base.OnYes();
        UIManager.Instance.ActivateInteractText(base.AfterYesMessage);
    }
}
