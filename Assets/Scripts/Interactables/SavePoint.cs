using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : InteractableObject {

    public override void OnInteract() {
        GameManager.Instance.SavePlayerData(gameObject.name);
    }
}