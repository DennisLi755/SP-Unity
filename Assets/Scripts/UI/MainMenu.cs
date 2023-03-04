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
    private Button startButton;
    [SerializeField]
    private Button fileOneButton;
    [SerializeField]
    private EventSystem es;
    [SerializeField]
    private GameObject fileButtons;
    private List<Button> files = new List<Button>();
    void Start() {
        for (int i = 0; i < fileButtons.transform.childCount-1; i++) {
            int x = i;
            fileButtons.transform.GetChild(i).transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { LoadSaveFile(x); });
        }
    }

    public void OpenLoadMenu() {
        titleMenu.SetActive(false);
        loadMenu.SetActive(true);
        es.SetSelectedGameObject(fileOneButton.gameObject);
    }

    public void OpenTitleMenu() {
        titleMenu.SetActive(true);
        loadMenu.SetActive(false);
        es.SetSelectedGameObject(startButton.gameObject);
    }

    public void LoadSaveFile(int index) {
        Debug.Log(index);
        SceneManager.LoadScene(1);
    }

    public void OptionsMenu() {

    }

    public void Quit() {
        Application.Quit();
    }
}
