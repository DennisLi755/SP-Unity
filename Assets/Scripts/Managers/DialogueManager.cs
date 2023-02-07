using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour {

    DialogueRunner dialogueRunner;
    [SerializeField]
    Canvas dialogueCanvas;

    void Start() {
        dialogueRunner = GetComponent<DialogueRunner>();
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
        dialogueCanvas.gameObject.SetActive(value);
    }
}