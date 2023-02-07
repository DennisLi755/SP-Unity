using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour {

    [SerializeField]
    private EventSystem es;
    [SerializeField]
    private GameObject dialogueSystem;

    void Start() {
        
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            StartDialogue();
        }
    }

    [ContextMenu("Get Selected")]
    public void GetSelected() {
        Debug.Log(es.currentSelectedGameObject);
    }

    [ContextMenu("Start Dialogue")]
    public void StartDialogue() {
        PlayerInfo.Instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        dialogueSystem.SetActive(true);
    }
}