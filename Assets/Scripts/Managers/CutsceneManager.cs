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
    private static CutsceneManager instance;
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
        if (!GameManager.Instance.WatchedOpening) {
            StartCoroutine(WaitForCrash());
            PlayerInfo.Instance.PlayerControl.Freeze();
        }
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

    [YarnCommand("watched_opening")]
    static void WatchedOpening() {
        GameManager.Instance.WatchedOpening = true;
    }
    /*
    [YarnCommand("show_dialogue")]
    static void ShowDialogueCanvas(bool show) {
        dialogueCanvas.SetActive(show);
    }*/
    [YarnCommand("move_object")]
    static void MoveObject(GameObject obj, float x, float y, float time, bool stopDialogue = true) {
        if (stopDialogue) {
            DialogueManager.Instance.EnableDialogueCanvas(false);
        }
        float moveX = obj.transform.position.x + x;
        float moveY = obj.transform.position.y + y;
        Vector3 destination = new Vector3(moveX, moveY, 0);

        float speed = Vector3.Distance(obj.transform.position, destination)/time;
        instance.StartCoroutine(Moving());

        IEnumerator Moving() {
            while (obj.transform.position != destination) {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, destination, speed * Time.deltaTime);
                yield return null;
                Debug.Log("In loop");
            }
            if (stopDialogue) {
                DialogueManager.Instance.EnableDialogueCanvas(true);
            }
        }
    }
    [YarnCommand("move_player")]
    static void MovePlayer(float x, float y, float time, bool stopDialogue = true) {
        if (stopDialogue) {
            DialogueManager.Instance.EnableDialogueCanvas(false);
            instance.StartCoroutine(Wait());
        }
        Vector3 playerPosition = PlayerInfo.Instance.gameObject.transform.position;
        Vector3 destination = new Vector3(playerPosition.x + x, playerPosition.y + y, 0);

        float speed = Vector3.Distance(playerPosition, destination)/time;
        float angle = Mathf.Atan2(destination.y - playerPosition.y, 
            destination.x  - playerPosition.x);
        PlayerInfo.Instance.PlayerControl.Velocity = new Vector2(speed*Mathf.Cos(angle * Mathf.Deg2Rad), speed*Mathf.Sin(angle * Mathf.Deg2Rad));

        IEnumerator Wait() {
            yield return new WaitForSeconds(time);
            PlayerInfo.Instance.PlayerControl.Velocity = Vector2.zero;
            DialogueManager.Instance.EnableDialogueCanvas(true);
        }
    }
}