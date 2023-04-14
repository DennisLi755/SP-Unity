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
    new void Start() {
        base.Start();
        if (GameManager.Instance.GetProgressionFlag("Sword Unlocked")) {
            tileMap.SetTile(position, null);
        }
    }
    /// <summary>
    /// OnInteract() for interacting with the sword
    /// If the sword has been unlocked do a regular object interaction
    /// If not, do the prompted interaction
    /// </summary>
    public override void OnInteract()
    {
        if (PlayerInfo.Instance.PlayerControl.AttackUnlocked == false) {
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
        PlayerInfo.Instance.AttackUnlocked = true;
        //code to remove sword sprite from world
        tileMap.SetTile(position, null);
        base.OnYes();
        UIManager.Instance.ActivateInteractText(base.AfterYesMessage);
        GameManager.Instance.SetProgressionFlag("Sword Unlocked", true);
        UIManager.Instance.EndInteractionEvent += () => { PlayerInfo.Instance.EnableTutorialText("Press Z to attack", "Press X to attack"); };
    }
}
