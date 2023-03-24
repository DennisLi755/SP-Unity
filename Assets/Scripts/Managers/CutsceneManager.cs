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
        
    }

    void Update() {
        
    }

    [YarnCommand("play_sound")]
    static void PlaySound(string sound) {
        SoundManager.Instance.PlaySoundEffect(sound, SoundSource.cutscene);
    }
    /*
    [YarnCommand("show_dialogue")]
    static void ShowDialogueCanvas(bool show) {
        dialogueCanvas.SetActive(show);
    }*/
}