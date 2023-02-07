using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour {

    private DialogueManager instance;
    public DialogueManager Instance => instance;

    private DialogueRunner dialogueRunner;
    private GameObject dialogueCanvas;
    private Image leftCharacter;
    private Image rightCharacter;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(transform.parent);
        }
        else {
            Destroy(this);
        }
    }

    void Start() {
        dialogueRunner = GetComponent<DialogueRunner>();
        dialogueCanvas = transform.GetChild(0).gameObject;
        leftCharacter = dialogueCanvas.transform.GetChild(0).GetComponent<Image>();
        rightCharacter = dialogueCanvas.transform.GetChild(1).GetComponent<Image>();
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