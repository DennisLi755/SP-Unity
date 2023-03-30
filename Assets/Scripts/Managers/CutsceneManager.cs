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
        DialogueManager.Instance.StartDialogue("First_Outside");
    }

    public void OpeningScene(Scene s, LoadSceneMode lsm) {
        OpeningScene();
    }
    public void OpeningScene() {
        if (!GameManager.Instance.GetProgressionFlag("Watched Opening") && SceneManager.GetActiveScene().buildIndex == 1) {
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
    [YarnCommand("set_progression_flag")]
    static void SetProgressionFlag(string key, bool bol) {
        GameManager.Instance.SetProgressionFlag(key, bol);
    }
    /*
    [YarnCommand("show_dialogue")]
    static void ShowDialogueCanvas(bool show) {
        dialogueCanvas.SetActive(show);
    }*/
    [YarnCommand("move_object")]
    static void MoveObject(GameObject obj, float x, float y, float time, bool relative = true, bool stopDialogue = true) {
        if (stopDialogue) {
            DialogueManager.Instance.EnableDialogueCanvas(false);
        }
        float moveX = relative ? obj.transform.position.x + x : x;
        float moveY = relative ? obj.transform.position.y + y : y;
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
        Debug.Log(speed*Mathf.Sin(angle));
        PlayerInfo.Instance.PlayerControl.Velocity = new Vector2((speed*Mathf.Cos(angle))/PlayerInfo.Instance.PlayerControl.ActiveMoveSpeed, 
            (speed*Mathf.Sin(angle))/PlayerInfo.Instance.PlayerControl.ActiveMoveSpeed);

        IEnumerator Wait() {
            yield return new WaitForSeconds(time);
            PlayerInfo.Instance.PlayerControl.Velocity = Vector2.zero;
            DialogueManager.Instance.EnableDialogueCanvas(true);
        }
    }
    [YarnCommand("change_player_direction")]
    static void ChangePlayerDirection(int direction) {
        PlayerInfo.Instance.PlayerControl.ForceDirection((FacingDirection)direction);
    }
    [YarnCommand("player_collision")]
    static void PlayerCollision(bool condition) {
        PlayerInfo.Instance.PlayerControl.CanCollide = condition;
    }
    [YarnCommand("fade_to_black")]
    static void FadeToBlack() {
        UIManager.Instance.FadeToBlack();
    }
    [YarnCommand("fade_from_black")]
    static void FadeFromBlack() {
        UIManager.Instance.FadeFromBlack();
    }
    [YarnCommand("cut_from_black")]
    static void CutFromBlack() {
        UIManager.Instance.CutFromBlack();
    }
    [YarnCommand("set_object_position")]
    static void SetObjectPosition(GameObject obj, float x, float y) {
        obj.transform.position = new Vector3(x, y, 0);
    }
    [YarnCommand("set_alpha")]
    static void SetAlpha(GameObject obj, float alpha) {
        //obj.SetActive(condition);
        Color c;
        c = Color.white;
        c.a = alpha;
        obj.GetComponent<SpriteRenderer>().color = c;
    }
    [YarnCommand("enable_textboxes")]
    static void EnableTextboxes(bool condition) {
        DialogueManager.Instance.EnableDialogueCanvas(condition);
    }
}