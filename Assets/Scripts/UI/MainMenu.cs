using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Assets;
using TMPro;

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
    private PlayerSaveData[] saveFiles = new PlayerSaveData[3];
    void Start() {
        SoundManager.Instance.SetUpMusicLayers(new string[]{"spdemo2"});
        for (int i = 0; i < fileButtons.transform.childCount-1; i++) {
            int x = i;
            Transform currButton = fileButtons.transform.GetChild(i);
            currButton.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { LoadSaveFile(x); });
            string filePath = Application.persistentDataPath + $"/save{i}.data";
            if (System.IO.File.Exists(filePath)) {
                saveFiles[i] = JsonUtility.FromJson<PlayerSaveData>(System.IO.File.ReadAllText(filePath));
                //fill in save data on UI
                //name
                currButton.GetChild(1).GetComponent<TMP_Text>().text = saveFiles[i].playerName;
                //save location
                currButton.GetChild(2).GetComponent<TMP_Text>().text = saveFiles[i].saveLocation.Replace("Save Toilet - ", "");
            }
        }
        es.SetSelectedGameObject(startButton.gameObject);
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
        UIManager.Instance.FadeToBlackLong();
        StartCoroutine(LoadData(3.0f, index));
        es.gameObject.SetActive(false);
        SoundManager.Instance.FadeOutCurrentLayer(2.5f);
    }

    IEnumerator LoadData(float time, int index) {
        yield return new WaitForSeconds(time);
        //start a new game
        if (saveFiles[index] == null) {
            GameManager.Instance.LoadPlayerSaveData(index, new PlayerSaveData());
        }
        //load an existing save
        else {
            GameManager.Instance.LoadPlayerSaveData(index, saveFiles[index]);
        }
    }

    public void PlayUISound(string effect) {
        SoundManager.Instance.PlaySoundEffect(effect, SoundSource.UI);
    }

    public void OptionsMenu() {

    }

    public void Quit() {
        Application.Quit();
    }
}
