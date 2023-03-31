using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletInteraction : InteractableObject {

    [SerializeField]
    private Sprite closedToilet;
    [SerializeField]
    private Sprite openToilet;
    private bool isOpen = false;
    private SpriteRenderer sr;

    new void Start() {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
    }

    new void Update() {

    }

    public override void OnInteract() {
        isOpen = !isOpen;
        if (isOpen) {
            sr.sprite = openToilet;
        }
        else {
            sr.sprite = closedToilet;
        }
    }
}
