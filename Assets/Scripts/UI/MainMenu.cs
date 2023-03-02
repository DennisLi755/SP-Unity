using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject titleMenu;
    [SerializeField]
    private GameObject loadMenu;
    [SerializeField]
    private Button newGameButton;
    [SerializeField]
    private Button fileOneButton;
    [SerializeField]
    private EventSystem es;
    public void NewGame() {
        SceneManager.LoadSceneAsync("Enemy Testing");
    }

    public void OpenLoadMenu() {
        titleMenu.SetActive(false);
        loadMenu.SetActive(true);
        es.SetSelectedGameObject(fileOneButton.gameObject);
    }

    public void OpenTitleMenu() {
        titleMenu.SetActive(true);
        loadMenu.SetActive(false);
        es.SetSelectedGameObject(newGameButton.gameObject);
    }

    public void LoadSaveFile(int index) {
        
    }

    public void OptionsMenu() {

    }

    public void Quit() {
        Application.Quit();
    }
}
