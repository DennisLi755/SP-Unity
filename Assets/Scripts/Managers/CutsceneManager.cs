using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour {
    private static CutsceneManager instance;
    public static CutsceneManager Instance => Instance;

    [SerializeField]
    private GameObject dialogueCanvas;
    [SerializeField]
    YarnProject openingProject;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        SceneManager.sceneLoaded += OpeningScene;
    }

    public void OpeningScene(Scene s, LoadSceneMode lsm) {
        OpeningScene();
    }
    public void OpeningScene() {
        if (!GameManager.Instance.WatchedOpening && SceneManager.GetActiveScene().buildIndex == 1) {
            StartCoroutine(WaitForCrash());
            PlayerInfo.Instance.PlayerControl.Freeze();
        }
    }

    IEnumerator WaitForCrash() {
        yield return new WaitForSeconds(1.0f);
        DialogueManager.Instance.StartDialogue(openingProject, "Opening");
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

    [YarnCommand("watched_opening")]
    static void WatchedOpening() {
        GameManager.Instance.WatchedOpening = true;
    }
    /*
    [YarnCommand("show_dialogue")]
    static void ShowDialogueCanvas(bool show) {
        dialogueCanvas.SetActive(show);
    }*/
}