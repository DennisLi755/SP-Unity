using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private static UIManager instance;
    public TMP_Text interactText;
    TMP_Text interactTextContinue;
    List<string> interactTexts;
    int currentTextIndex;
    public static UIManager Instance {
        get {
            if (instance == null) {
                instance = new UIManager();
            }
            return instance;
        }
    }

    [SerializeField]
    Image fadeToBlackPanel;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {

    }

    void Update() {

    }

    [ContextMenu("FadeToBlack")]
    public void FadeToBlack() {
        FadeToBlack(true);
    }

    [ContextMenu("Fade From Black")]
    public void FadeFromBlack() {
        FadeToBlack(false);
    }

    private void FadeToBlack(bool fade) {
        if (fade) {
            fadeToBlackPanel.GetComponent<Animation>().Play("FadeToBlack");
        }
        else {
            fadeToBlackPanel.GetComponent<Animation>().Play("FadeFromBlack");
        }
    }

    public void ActivateInteractText(List<string> message) {
        interactTexts = message;
        currentTextindex = 0;

        interactText.SetText(interactTexts[currentTextindex++]);

        interactText.transform.parent.gameObject.SetActive(true);

        PlayerInfo.Instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
    }

    public void ContinueInteractText() {
        if (currentTextIndex == interactTexts.Length) {
            DeactivateInteractText();
        } else {
            interactText.SetText(interactTexts[currentTextIndex++]);
        }
    }

    public void DeactivateInteractText() {
        interactText.transform.parent.gameObject.SetActive(false);
        PlayerInfo.Instance.GetComponent<PlayerInfo>().SwitchCurrentActionMap("Player");
    }
}