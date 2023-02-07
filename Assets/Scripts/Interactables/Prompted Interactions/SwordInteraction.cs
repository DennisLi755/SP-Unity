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
    /// <summary>
    /// OnInteract() for interacting with the sword
    /// If the sword has been unlocked do a regular object interaction
    /// If not, do the prompted interaction
    /// </summary>
    public override void OnInteract()
    {
        if (PlayerInfo.Instance.PlayerControl.UnlockAttack == false) {
            base.OnInteract();
        } else {
            UIManager.Instance.ActivateInteractText(new List<string>{"Just a regular coffee table."});
        }
    }
    /// <summary>
    /// OnYes() for interacting with the sword
    /// Update the tile map and data within PlayerControl
    /// Show Interact text after yes
    /// </summary>
    public override void OnYes()
    {
        PlayerInfo.Instance.PlayerControl.UnlockAttack = true;
        //code to remove sword sprite from world
        tileMap.SetTile(position, null);
        base.OnYes();
        UIManager.Instance.ActivateInteractText(base.AfterYesMessage);
    }
}
