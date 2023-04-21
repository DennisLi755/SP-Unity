using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreatInteractable : CutsceneInteractable {
    public override void OnInteract() {
        base.OnInteract();
        gameObject.SetActive(false);
        PlayerInfo.Instance.ExitInteractable(this);
    }
}