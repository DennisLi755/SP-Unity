using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Manages overall Game information such as SP value, important player progression flags, etc.
/// Manages the persistent information of these variables upon save and load
/// </summary>
public class GameManager : MonoBehaviour {

    private static GameManager instance;
    public static GameManager Instance => instance;

    private bool isPaused;
    public bool IsPaused => isPaused;

    private int playerSaveSlot;
    public int PlayerSaveSlot { set => playerSaveSlot = value; }
    private PlayerSaveData saveData;
    public PlayerSaveData SaveData { get => saveData; set => saveData = value; }
    private string SaveFilePath => Application.persistentDataPath + $"/save{playerSaveSlot}.data";

    //Progression flags
    private Dictionary<string, bool> progressionFlags = new Dictionary<string, bool>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        playerSaveSlot = 0;
    }

    void Update() {
#if UNITY_EDITOR
        //Dev commands used for easy testing
        //Pauses the game without bringing up the menu so you can still see the current state of the scnree
        if (Input.GetKeyDown(KeyCode.P)) {
            Time.timeScale = Mathf.Abs(Time.timeScale - 1.0f);
        }
        //Dev Immunity
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            PlayerInfo.Instance.Invinicble = !PlayerInfo.Instance.Invinicble;
        } 
        //Saves the game without needing to interact wtih a save toilet
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            SavePlayerData("Debug");
        }
        //Loads save data without having to go through the main menu
        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            PlayerSaveData file;
            string filePath = Application.persistentDataPath + $"/save{playerSaveSlot}.data";
            file = JsonUtility.FromJson<PlayerSaveData>(System.IO.File.ReadAllText(filePath));
            LoadPlayerSaveData(playerSaveSlot, file);
        }
#endif
    }

    /// <summary>
    /// Toggles the game's pause state
    /// </summary>
    public void PauseGame() {
        if (DialogueManager.Instance.IsDialogueRunning) {
            return;
        }
        isPaused = !isPaused;
        UIManager.Instance.ToggleMenu();
        //pausing the game
        if (isPaused) {
            Time.timeScale = 0.0f;
        }
        //unpausing the game
        else {
            Time.timeScale = 1.0f;
        }
    }

    /// <summary>
    /// Returns whether the player has completed a given progression flag
    /// If the flag does not exist in the dictionary already, then it is assumed the player has not completed and it is added for future calls
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool GetProgressionFlag(string key) {
        try {
            return progressionFlags[key];
        }
        catch (KeyNotFoundException) {
            progressionFlags.Add(key, false);
            return false;
        }
    }

    /// <summary>
    /// Sets the value of a progression flag
    /// </summary>
    /// <param name="key"></param>
    /// <param name="bol"></param>
    public void SetProgressionFlag(string key, bool bol) {
        progressionFlags[key] = bol;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void EndFight() {
        StartCoroutine(EndDemoFight());
    }

    IEnumerator EndDemoFight() {
        UIManager.Instance.FadeToBlack();

        yield return new WaitForSecondsRealtime(1.0f);
        //Time.timeScale = 0.0f;
        BulletHolder.Instance.ClearBullets();
        PlayerInfo.Instance.CombatLock = false;
        SoundManager.Instance.ResetMusicLayers();
        PlayerInfo.Instance.ExitCombat();
        SceneManager.LoadScene(0);

        Time.timeScale = 1.0f;
        UIManager.Instance.FadeFromBlack();
    }

    /// <summary>
    /// Serailzes the player's data into a save file
    /// </summary>
    /// <param name="saveLocation"></param>
    public void SavePlayerData(string saveLocation) {
        //generate a saveData object and fill in the info
        saveData = new PlayerSaveData();
        saveData.scene = SceneManager.GetActiveScene().name;
        saveData.saveLocation = saveLocation;
        saveData.unlockedSkills = PlayerInfo.Instance.PlayerControl.UnlockedSkills;
        saveData.equippedSkills = PlayerInfo.Instance.PlayerControl.EquippedSkills;
        saveData.FillProgressionFlags(progressionFlags);
        saveData.FillTutorialFlags(PlayerInfo.Instance.Tutorials);
        //write the data to a persistent file
        System.IO.File.WriteAllText(SaveFilePath, JsonUtility.ToJson(saveData));
        Debug.Log("Saved game!");
    }

    /// <summary>
    /// Loads the scene saved in the save data and restores progression flags
    /// </summary>
    /// <param name="saveSlot"></param>
    /// <param name="psd"></param>
    public void LoadPlayerSaveData(int saveSlot, PlayerSaveData psd) {
        saveData = psd;
        playerSaveSlot = saveSlot;

        //load the correct scene if it is not already loaded
        progressionFlags = saveData.GetProgressionFlags();
        SceneManager.sceneLoaded += SceneLoaded;

        if (!new System.IO.FileInfo(SaveFilePath).Exists) {
            //Make a new save
            Debug.Log($"There is no save data in slot {playerSaveSlot}");
            SceneManager.LoadScene(1);
        }
        else {
            SceneManager.LoadScene(saveData.scene);
        }
    }

    /// <summary>
    /// In-Between method called once the SceneManager finishes loading the correct scene
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="lsm"></param>
    private void SceneLoaded(Scene scene, LoadSceneMode lsm) {
        StartCoroutine(SetupPlayer());
        //remove the event in case we load a scene during gameplay that is not based on save data
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    /// <summary>
    /// Setups the player's data and position based on the current save data
    /// </summary>
    IEnumerator SetupPlayer() {
        yield return null;
        UIManager.Instance.UpdatePlayerHealth(1.0f);
        //get the player
        PlayerControl player = PlayerInfo.Instance.PlayerControl;
        Dictionary<string, bool> tutorialFlags = saveData.GetTutorialFlags();
        if (tutorialFlags.Count != 0) {
            PlayerInfo.Instance.Tutorials = saveData.GetTutorialFlags();
        }
        //unlock all skills listed in the save data
        foreach (int skillID in saveData.unlockedSkills) {
            player.UnlockSkill(skillID);
        }
        //equip the correct skills in their coresponding slots
        player.EquipSkill(saveData.equippedSkills[0], 0);
        player.EquipSkill(saveData.equippedSkills[1], 1);
        //move the player to the correct save location in the scene
        if (saveData.saveLocation != "Debug") {
            SavePoint[] sceneSavePoints = FindObjectsOfType<SavePoint>();
            foreach (SavePoint sp in sceneSavePoints) {
                if (sp.gameObject.name == saveData.saveLocation) {
                    if (GetProgressionFlag("Sword Unlocked")) {
                        PlayerInfo.Instance.AttackUnlocked = true;
                    }
                    player.HitboxPosition = sp.PlayerPosition;
                    //move the camera to the room that the save point is in - parent is called twice because save
                    //points are children of interactables which are children of the room
                    sp.transform.parent.parent.GetComponent<Room>().MoveCameraHere();
                    sp.transform.parent.parent.GetComponent<Room>().onEnter?.Invoke();
                    break;
                }
            }
        }
        else {
            GameObject.Find("Living Room").GetComponent<Room>().MovePlayerHere();
            GameObject.Find("Living Room").GetComponent<Room>().MoveCameraHere();
        }
        yield return null;
        UIManager.Instance.FadeFromBlack();
    }

    [ContextMenu("Fix Interactables")]
    public void UpdateInteractables() {
        InteractableObject[] objs = FindObjectsOfType<InteractableObject>();
        foreach (InteractableObject io in objs) {
           
        }
    }

    public static void MarkSceneDirty() {
        if (Application.isPlaying) {
            return;
        }
#if UNITY_EDITOR
        EditorSceneManager.MarkAllScenesDirty();
#endif
    }
}