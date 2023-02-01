using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSwitch : InteractableObject {

    private bool lightOn;
    [SerializeField]
    private GameObject lightObject;

    private void Start() {
        base.Start();
        lightOn = lightObject.activeSelf;
    }

    public override void OnInteract() {
        lightOn = !lightOn;
        lightObject.SetActive(lightOn);
    }
}