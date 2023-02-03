using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordInteraction : PromptedInteraction
{
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
        base.OnYes();
        UIManager.Instance.ActivateInteractText(base.AfterYesMessage);
    }
}
