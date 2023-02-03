using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSwitch : InteractableObject {

    private bool lightOn;
    [SerializeField]
    private GameObject lightObject;

    private new void Start() {
        base.Start();
        lightOn = lightObject.activeSelf;
    }

    public override void OnInteract() {
        if (PlayerInfo.Instance.PlayerControl.FacingDirection == FacingDirection.Up) {
            lightOn = !lightOn;
            lightObject.SetActive(lightOn);
        }
    }
}