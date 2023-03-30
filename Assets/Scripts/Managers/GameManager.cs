using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (Input.GetKeyDown(KeyCode.P)) {
            Time.timeScale = Mathf.Abs(Time.timeScale - 1.0f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            EndFight();
        } 
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            SavePlayerData("Debug");
        }
    }

    public void PauseGame() {
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

    public bool GetProgressionFlag(string key) {
        try {
            return progressionFlags[key];
        }
        catch (KeyNotFoundException e) {
            progressionFlags.Add(key, false);
            return false;
        }
    }

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
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
        PlayerInfo.Instance.ExitCombat();
        UIManager.Instance.FadeFromBlack();
    }

    public void SavePlayerData(string saveLocation) {
        //generate a saveData object and fill in the info
        saveData = new PlayerSaveData();
        saveData.scene = SceneManager.GetActiveScene().name;
        saveData.saveLocation = saveLocation;
        saveData.unlockedSkills = PlayerInfo.Instance.PlayerControl.UnlockedSkills;
        saveData.equippedSkills = PlayerInfo.Instance.PlayerControl.EquippedSkills;
        saveData.progressionFlags = progressionFlags;
        //write the data to a persistent file
        System.IO.File.WriteAllText(SaveFilePath, JsonUtility.ToJson(saveData));
        Debug.Log("Saved game!");
    }

    public void LoadPlayerSaveData(int saveSlot, PlayerSaveData psd) {
        saveData = psd;
        playerSaveSlot = saveSlot;

        //load the correct scene if it is not already loaded
        progressionFlags = saveData.progressionFlags;
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


    private void SceneLoaded(Scene scene, LoadSceneMode lsm) {
        SetupPlayer();
        SceneManager.sceneLoaded -= SceneLoaded;
        
    }

    private void SetupPlayer() {
        //get the player
        PlayerControl player = PlayerInfo.Instance.PlayerControl;
        //unlock all skills listed in the save data
        foreach (int skillID in saveData.unlockedSkills) {
            player.UnlockSkill(skillID);
        }
        //equip the correct skills in their coresponding slots
        player.EquipSkill(saveData.equippedSkills[0], 0);
        player.EquipSkill(saveData.equippedSkills[1], 1);
        //move the player to the correct save location in the scene
        SavePoint[] sceneSavePoints = FindObjectsOfType<SavePoint>();
        foreach (SavePoint sp in sceneSavePoints) {
            if (sp.gameObject.name == saveData.saveLocation) {
                player.transform.position = sp.PlayerPosition;
                //move the camera to the room that the save point is in - parent is called twice because save points are children of interactables which are children of the room
                sp.transform.parent.parent.GetComponent<Room>().MoveCameraHere();
                break;
            }
        }
        UIManager.Instance.FadeFromBlack();
    }
}