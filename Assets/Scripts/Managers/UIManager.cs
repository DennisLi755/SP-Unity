using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour {

    private static UIManager instance;
    public TMP_Text interactText;
    List<string> interactTexts;
    int currentTextIndex;
    [SerializeField]
    Button promptedInteractionYesButton;
    [SerializeField]
    Button textInteractionButton;
    [SerializeField]
    EventSystem eventSystem;
    [SerializeField]
    GameObject prompt;
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
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {

    }

    void Update() {

    }

    /// <summary>
    /// Fades the screen to black
    /// </summary>
    [ContextMenu("FadeToBlack")]
    public void FadeToBlack() {
        FadeToBlack(true);
    }

    /// <summary>
    /// Fades the screen from black
    /// </summary>
    [ContextMenu("Fade From Black")]
    public void FadeFromBlack() {
        FadeToBlack(false);
    }

    /// <summary>
    /// Controls whether the screen fades to or from black based on the paramter
    /// </summary>
    /// <param name="fade">true causes the screen to fade to black; false causes the screen to from from black</param>
    private void FadeToBlack(bool fade) {
        if (fade) {
            fadeToBlackPanel.GetComponent<Animation>().Play("FadeToBlack");
        }
        else {
            fadeToBlackPanel.GetComponent<Animation>().Play("FadeFromBlack");
        }
    }

    public void ActivateInteractText(List<string> message) {
        eventSystem.firstSelectedGameObject = textInteractionButton.gameObject;
        eventSystem.SetSelectedGameObject(textInteractionButton.gameObject);
        //textInteractionButton.onClick.AddListener(ContinueInteractText);
        interactTexts = message;
        currentTextIndex = 0;

        interactText.SetText(interactTexts[currentTextIndex++]);

        interactText.transform.parent.gameObject.SetActive(true);

        PlayerInfo.Instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
    }

    public void ContinueInteractText() {
        if (currentTextIndex == interactTexts.Count) {
            DeactivateInteractText();
        } else {
            interactText.SetText(interactTexts[currentTextIndex++]);
        }
    }

    public void DeactivateInteractText() {
        interactText.transform.parent.gameObject.SetActive(false);
        PlayerInfo.Instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
    }

    public void ActivatePromptedInteraction(PromptedInteraction sender, List<string> message) {
        eventSystem.firstSelectedGameObject = promptedInteractionYesButton.gameObject;
        eventSystem.SetSelectedGameObject(promptedInteractionYesButton.gameObject);
        promptedInteractionYesButton.onClick.AddListener(sender.OnYes);
        interactTexts = message;
        currentTextIndex = 0;

        interactText.SetText(interactTexts[currentTextIndex]);

        interactText.transform.parent.gameObject.SetActive(true);
        prompt.SetActive(true);

        PlayerInfo.Instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
    }

    public void ResetPromptedInteraction() {
        interactText.transform.parent.gameObject.SetActive(false);
        promptedInteractionYesButton.onClick.RemoveAllListeners();
        prompt.SetActive(false);
        PlayerInfo.Instance.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
    }
}