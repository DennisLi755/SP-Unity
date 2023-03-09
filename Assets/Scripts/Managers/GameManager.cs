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

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {

    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            Time.timeScale = Mathf.Abs(Time.timeScale - 1.0f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            EndFight();
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

    public void QuitGame() {
        Application.Quit();
    }

    public void EndFight() {
        StartCoroutine(EndDemoFight());
    }

    IEnumerator EndDemoFight() {
        UIManager.Instance.FadeToBlack();
        yield return new WaitForSeconds(0.5f);
        GuideBoss guideboss = FindObjectOfType<GuideBoss>();
        if (guideboss != null) {
            Destroy(guideboss.gameObject);
        }
        GuideAd[] ads = FindObjectsOfType<GuideAd>();
        for (int i = 0; i < ads.Length; Destroy(ads[i].gameObject), i++);

        Bullet[] bs = FindObjectsOfType<Bullet>();
        for (int i = 0; i < bs.Length; Destroy(bs[i].gameObject), i++);

        PlayerInfo.Instance.Respawn();
        PlayerInfo.Instance.PlayerControl.CanMove = false;
        PlayerInfo.Instance.transform.position = new Vector3(104.2f, -32.1f, 0.0f);
        PlayerInfo.Instance.ExitCombat();

        UIManager.Instance.FadeFromBlack();
        yield return new WaitForSeconds(0.5f);
        PlayerInfo.Instance.PlayerControl.CanMove = true;
    }

    public void SavePlayerData(string saveLocation) {
        PlayerSaveData saveData = new PlayerSaveData();
        saveData.scene = SceneManager.GetActiveScene().name;
        saveData.saveLocation = saveLocation;
    }

    public void LoadPlayerSaveData() {
        //read in data from correct save file;
        PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>("");

        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.LoadScene(saveData.scene);
    }

    private void SceneLoaded(Scene scene, LoadSceneMode lsm) {
        
    }
}