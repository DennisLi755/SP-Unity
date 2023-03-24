using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Unity;

public class CutsceneManager : MonoBehaviour {
    [SerializeField]
    private GameObject dialogueCanvas;

    void Start() {
        StartCoroutine(WaitForCrash());
        PlayerInfo.Instance.PlayerControl.Freeze();
    }

    IEnumerator WaitForCrash() {
        yield return new WaitForSeconds(1.0f);
        DialogueManager.Instance.StartDialogue("Opening");
    }

    void Update() {
        
    }

    [YarnCommand("play_sound")]
    static void PlaySound(string sound) {
        SoundManager.Instance.PlaySoundEffect(sound, SoundSource.cutscene);
    }

    [YarnCommand("show_movement_text")]
    static void ShowMovementText() {
        if (PlayerInfo.Instance.GetComponent<PlayerInput>().currentControlScheme == "Keyboard") {
            PlayerInfo.Instance.EnableTutorialText("Use the arrow keys to move");
        }
        else {
            PlayerInfo.Instance.EnableTutorialText("Use the the left stick to move");
        }
    }

    [YarnCommand("talk_to_guide")]
    static void TalkedToGuide() {
        GameManager.Instance.GuideInteract = true;
    }
    /*
    [YarnCommand("show_dialogue")]
    static void ShowDialogueCanvas(bool show) {
        dialogueCanvas.SetActive(show);
    }*/
}