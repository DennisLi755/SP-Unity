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
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    public override void OnInteract()
    {
        base.OnInteract();
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
