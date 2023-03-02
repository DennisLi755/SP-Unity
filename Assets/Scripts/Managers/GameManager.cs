using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages overall Game information such as SP value, important player progression flags, etc.
/// Manages the persistent information of these variables upon save and load
/// </summary>
public class GameManager : MonoBehaviour {

    private static GameManager instance;
    public static GameManager Instance => instance;

    private bool isPaused;
    public bool IsPaused => isPaused;

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
}