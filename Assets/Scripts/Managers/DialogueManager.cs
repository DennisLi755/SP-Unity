using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour {

    DialogueRunner dialogueRunner;
    GameObject dialogueCanvas;

    void Start() {
        dialogueRunner = GetComponent<DialogueRunner>();
        dialogueCanvas = transform.GetChild(0).gameObject;
    }

    void Update() {

    }

    public void DisableDialogueCanvas() {
        EnableDialogueCanvas(false);
    }

    public void EnableDialogueCanvas() {
        EnableDialogueCanvas(true);
    }

    private void EnableDialogueCanvas(bool value) {
        dialogueCanvas.SetActive(value);
    }
}