using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame() {
        SceneManager.LoadSceneAsync("Enemy Testing");
    }

    public void LoadGame() {

    }

    public void OptionsMenu() {

    }

    public void Quit() {
        Application.Quit();
    }
}
