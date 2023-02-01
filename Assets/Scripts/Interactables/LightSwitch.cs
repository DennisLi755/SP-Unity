using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSwitch : InteractableObject {

    private bool lightOn = false;
    [SerializeField]
    private Image lightObject;

    private void Start() {
        base.Start();
    }

    private void Update() {
        base.Update();
    }

    public override void OnInteract() {
        lightOn = !lightOn;
        Color color = lightObject.color;
        color.a = lightOn ? 0.0f : 0.5f;
        lightObject.color = color;
    }
}