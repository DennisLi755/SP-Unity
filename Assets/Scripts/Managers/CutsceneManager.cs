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
[Serializable]
public struct Objs {
    public string name;
    public GameObject obj;
}

public class CutsceneManager : MonoBehaviour {
    private static CutsceneManager instance;
    public static CutsceneManager Instance => Instance;

    [SerializeField]
    private Objs[] objsArray;
    private static Dictionary<string, GameObject> objs;
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
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            SceneManager.sceneLoaded += OpeningScene;
        }
        else {
            OpeningScene(new Scene(), 0);
        }
        
        objs = new Dictionary<string, GameObject>();
        foreach (Objs o in objsArray) {
            objs.Add(o.name, o.obj);
        }
    }

    public void OpeningScene(Scene s, LoadSceneMode lsm) {
        if (SceneManager.GetActiveScene().buildIndex == 1) {
            if (!GameManager.Instance.GetProgressionFlag("Watched Opening")) {
                OpeningScene();
            }
            else if (GameManager.Instance.GetProgressionFlag("Second Awake")) {
                GameObject guide = Instantiate(objs["Guide Crash"], new Vector3(96.02f, -38.73f, 0.0f), Quaternion.identity);
                guide.name = guide.name.Replace("(Clone)", "").Trim();
            }
        }
    }

    public void OpeningScene() {
        DialogueManager.Instance.StartDialogue(openingProject, "Opening");
        PlayerInfo.Instance.PlayerControl.Freeze();
    }

    #region Sounds & Music
    /// <summary>
    /// Plays the soud effect with the coresponding name on the cutscene audio source
    /// </summary>
    /// <param name="sound"></param>
    [YarnCommand("play_sound")]
    static void PlaySound(string sound) {
        SoundManager.Instance.PlaySoundEffect(sound, SoundSource.cutscene);
    }

    /// <summary>
    /// Sets up music layers
    /// </summary>
    /// <param name="layers">the string of layers for spliting</param>
    [YarnCommand("music_layers")]
    static void SetUpMusicLayers(string layers) {
        string[] musicLayers = layers.Split(',');
        SoundManager.Instance.SetUpMusicLayers(musicLayers);
    }

    /// <summary>
    /// Changes the layer of the current song
    /// </summary>
    [YarnCommand("change_music_layer")]
    static void ChangeMusicLayer() {
        SoundManager.Instance.ChangeMusicLayer(3f);
    }

    /// <summary>
    /// Fades out the current music layer
    /// </summary>
    /// <param name="time"></param>
    [YarnCommand("fade_music_layer")]
    static void FadeMusicLayer(float time) {
        SoundManager.Instance.FadeOutCurrentLayer(time);
    }


    #endregion

    #region Object Control
    /// <summary>
    /// Moves a gameObject to a new position in the scene
    /// </summary>
    /// <param name="obj">The Object to move</param>
    /// <param name="x">The new x position</param>
    /// <param name="y">The new y position</param>
    /// <param name="time">The time which the objects takes to move</param>
    /// <param name="relative">Whether the movement is relative to its current positoin</param>
    [YarnCommand("move_object")]
    static void MoveObject(GameObject obj, float x, float y, float time, bool relative = true) {
        float moveX = relative ? obj.transform.position.x + x : x;
        float moveY = relative ? obj.transform.position.y + y : y;
        Vector3 destination = new Vector3(moveX, moveY, 0);

        float speed = Vector3.Distance(obj.transform.position, destination) / time;
        instance.StartCoroutine(Moving());

        IEnumerator Moving() {
            while (obj.transform.position != destination) {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    /// <summary>
    /// Move a game object to a specific position in the scene
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    [YarnCommand("set_object_position")]
    static void SetObjectPosition(GameObject obj, float x, float y) {
        obj.transform.position = new Vector3(x, y, 0);
    }

    /// <summary>
    /// Changes the alpha of an object's sprite renderer
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="alpha"></param>
    [YarnCommand("set_alpha")]
    static void SetAlpha(GameObject obj, float alpha) {
        Color c;
        c = Color.white;
        c.a = alpha;
        obj.GetComponent<SpriteRenderer>().color = c;
    }

    /// <summary>
    /// Fades an object's alpha over time
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetAlpha"></param>
    /// <param name="time"></param>
    [YarnCommand("fade_object")]
    static void FadeObject(GameObject obj, float targetAlpha, float time) {
        instance.StartCoroutine(Fade());

        IEnumerator Fade() {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            float startingAlpha = sr.color.a;
            float t = 0.0f;
            while (t < time) {
                float newAlpha = Mathf.Lerp(startingAlpha, targetAlpha, t / time);
                Color newColor = sr.color;
                newColor.a = newAlpha;
                sr.color = newColor;
                t += Time.deltaTime;
                //wait a frame
                yield return null;
            }
            Color finalColor = sr.color;
            finalColor.a = targetAlpha;
            sr.color = finalColor;
        }
    }

    /// <summary>
    /// Destroys a GameObject
    /// </summary>
    /// <param name="obj">GameObject to destroy</param>
    [YarnCommand("destroy_object")]
    static void DestroyObject(GameObject obj) {
        Destroy(obj);
    }

    /// <summary>
    /// Creates a new GameObject at the given positoin in the scene
    /// </summary>
    /// <param name="name"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    [YarnCommand("instantiate")]
    static void MakeObject(string name, float x, float y) {
        GameObject obj = Instantiate(objs[name], new Vector3(x, y, 0f), Quaternion.identity);
        obj.name = "Guide";
    }
    /// <summary>
    /// Sets a new value for the Sorting Order property of a GameObjects Sprite Renderer
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layer"></param>
    [YarnCommand("object_layer")]
    static void ObjectLayer(GameObject obj, int layer) {
        obj.GetComponent<SpriteRenderer>().sortingOrder = layer;
    }


    #region Player
    /// <summary>
    /// Moves the player to a new position in the scene
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="time"></param>
    /// <param name="relative"></param>
    [YarnCommand("move_player")]
    static void MovePlayer(float x, float y, float time, bool relative = true) {
        instance.StartCoroutine(Wait());
        Vector3 playerPosition = PlayerInfo.Instance.gameObject.transform.position;
        float moveX = relative ? playerPosition.x + x : x;
        float moveY = relative ? playerPosition.y + y : y;
        Vector3 destination = new Vector3(moveX, moveY, 0);

        float speed = Vector3.Distance(playerPosition, destination) / time;
        //In order to have the player's anmiation change while moving without additional commands, we move the player by setting their velocity
        //This requires calculating direction they should move to a normalized angle vector
        float angle = Mathf.Atan2(destination.y - playerPosition.y,
            destination.x - playerPosition.x);

        PlayerInfo.Instance.PlayerControl.Velocity = new Vector2((speed * Mathf.Cos(angle)) / PlayerInfo.Instance.PlayerControl.ActiveMoveSpeed,
            (speed * Mathf.Sin(angle)) / PlayerInfo.Instance.PlayerControl.ActiveMoveSpeed);

        //After the appropriate time has passed, stop the player
        IEnumerator Wait() {
            yield return new WaitForSeconds(time);
            PlayerInfo.Instance.PlayerControl.Velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Change the direction is facing
    /// </summary>
    /// <param name="direction"></param>
    [YarnCommand("change_player_direction")]
    static void ChangePlayerDirection(int direction) {
        PlayerInfo.Instance.PlayerControl.ForceDirection((FacingDirection)direction);
    }

    /// <summary>
    /// Toggles the player's collidion calculations
    /// </summary>
    /// <param name="isEnabled"></param>
    [YarnCommand("player_collision")]
    static void PlayerCollision(bool isEnabled) {
        PlayerInfo.Instance.PlayerControl.CanCollide = isEnabled;
    }
    #endregion
    #endregion

    #region UI Control

    [ContextMenu("Movement Text")]
    public void MoveMentText() {
        ShowMovementText();
    }

    /// <summary>
    /// Shows the movement tutorial text over the player's head
    /// </summary>
    [YarnCommand("show_movement_text")]
    static void ShowMovementText() {
        PlayerInfo.Instance.EnableTutorialText("Use the arrow keys to move & SPACE to interact",
            "Use the the left stick to move & A to interact");
    }

    /// <summary>
    /// Fades the screen to black
    /// </summary>
    [YarnCommand("fade_to_black")]
    static void FadeToBlack() {
        UIManager.Instance.FadeToBlack();
    }

    /// <summary>
    /// Fade the screen from black
    /// </summary>
    [YarnCommand("fade_from_black")]
    static void FadeFromBlack() {
        UIManager.Instance.FadeFromBlack();
    }

    /// <summary>
    /// Cuts the screen from black
    /// </summary>
    [YarnCommand("cut_from_black")]
    static void CutFromBlack() {
        UIManager.Instance.CutFromBlack();
    }
    /// <summary>
    /// Fades the screen to white
    /// </summary>
    [YarnCommand("fade_to_white")]
    static void FadeToWhite() {
        UIManager.Instance.FadeToWhite();
    }
    /// <summary>
    /// Fades the screen from white
    /// </summary>
    [YarnCommand("fade_from_white")]
    static void FadeFromWhite() {
        UIManager.Instance.FadeFromWhite();
    }
    #endregion

    #region Progression Control
    /// <summary>
    /// Sets a progression flag for the player
    /// </summary>
    /// <param name="flagName"></param>
    /// <param name="value"></param>
    [YarnCommand("set_progression_flag")]
    static void SetProgressionFlag(string flagName, bool value) {
        GameManager.Instance.SetProgressionFlag(flagName, value);
    }

    /// <summary>
    /// Toggles the dialogue canvas's visbility
    /// </summary>
    /// <param name="isEnabled"></param>
    [YarnCommand("enable_textboxes")]
    static void EnableTextboxes(bool isEnabled) {
        DialogueManager.Instance.EnableDialogueCanvas(isEnabled);
    }

    /// <summary>
    /// 
    /// </summary>
    [YarnCommand("clear_bullets")]
    static void ClearBullets() {
        BulletHolder.Instance.ClearBullets();
    }
    #endregion

    #region Camera Control
    /// <summary>
    /// Sets a new target GameObject for the camera
    /// </summary>
    /// <param name="target">target gameobject</param>
    [YarnCommand("set_camera_target")]
    static void SetCameraTarget(GameObject target) {
        CameraManager.Instance.SetNewTarget(target.transform);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="time"></param>
    [YarnCommand("pan_camera")]
    static void PanCameraTo(float x, float y, float time) {
        CameraManager.Instance.PanTo(new Vector3(x, y, -10), time);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="time"></param>
    [YarnCommand("shake_camera")]
    static void ShakeCamera(float intensity, float time) {
        CameraManager.Instance.ScreenShake(intensity, time);
    }
    #endregion

    #region Animation Control
    /// <summary>
    /// Sets an animation trigger
    /// </summary>
    /// <param name="go">GameObject animation is attached to</param>
    /// <param name="trigger">the trigger name</param>
    [YarnCommand("set_anim_trigger")]
    static void SetAnimTrigger(GameObject go, string trigger) {
        go.GetComponent<Animator>().SetTrigger(trigger);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <param name="trigger"></param>
    [YarnCommand("reset_anim_trigger")]
    static void ResetAnimTrigger(GameObject go, string trigger) {
        go.GetComponent<Animator>().ResetTrigger(trigger);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    [YarnCommand("reset_all_anim_triggers")]
    static void ResetAllAnimTriggers(GameObject go) {
        Animator animator = go.GetComponent<Animator>();
        AnimatorControllerParameter[] acp = animator.parameters;
        foreach (AnimatorControllerParameter param in acp) {
            if (param.type == AnimatorControllerParameterType.Trigger) {
                animator.ResetTrigger(param.name);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <param name="bol"></param>
    /// <param name="value"></param>
    [YarnCommand("set_anim_bool")]
    static void SetAnimBool(GameObject go, string bol, bool value) {
        go.GetComponent<Animator>().SetBool(bol, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <param name="number"></param>
    /// <param name="value"></param>
    [YarnCommand("set_anim_int")]
    static void SetAnimInt(GameObject go, string number, int value) {
        go.GetComponent<Animator>().SetInteger(number, value);
    }
    #endregion
}