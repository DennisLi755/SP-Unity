using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour {
    #region field declaration
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
    Slider health;
    [SerializeField]
    private GameObject prompt;
    public GameObject Prompt {get => prompt; set {prompt = value;}}
    #endregion
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

    /// <summary>
    /// Opens and populates basic object interaction text box
    /// </summary>
    /// <param name="message">list of strings to show</param>
    public void ActivateInteractText(List<string> message) {
        //update selected button in event system
        textInteractionButton.gameObject.SetActive(true);
        eventSystem.firstSelectedGameObject = textInteractionButton.gameObject;
        eventSystem.SetSelectedGameObject(textInteractionButton.gameObject);
        
        //set the texts to be used for the interaction and enable the text UI object
        interactTexts = message;
        currentTextIndex = 0;
        interactText.SetText(interactTexts[currentTextIndex++]);
        interactText.transform.parent.gameObject.SetActive(true);

        //disable the player's movement
        PlayerInfo.Instance.ChangeInputMap("UI");
    }
    /// <summary>
    /// When the current messages finish either continue or end the interaction
    /// </summary>
    public void ContinueInteractText() {
        if (currentTextIndex == interactTexts.Count) {
            DeactivateInteractText();
        } else {
            interactText.SetText(interactTexts[currentTextIndex++]);
        }
    }
    /// <summary>
    /// End the current object interaction
    /// </summary>
    public void DeactivateInteractText() {
        interactText.transform.parent.gameObject.SetActive(false);
        PlayerInfo.Instance.ChangeInputMap("Player");
    }
    /// <summary>
    /// Opens and populates text box for a basic prompted object interaction
    /// Links yes button with the sender object's OnYes function
    /// </summary>
    /// <param name="sender">object that started the prompted interaction</param>
    /// <param name="message">list of strings to show</param>
    public void ActivatePromptedInteraction(PromptedInteraction sender, List<string> message) {
        textInteractionButton.gameObject.SetActive(false);
        eventSystem.firstSelectedGameObject = promptedInteractionYesButton.gameObject;
        eventSystem.SetSelectedGameObject(promptedInteractionYesButton.gameObject);
        promptedInteractionYesButton.onClick.AddListener(sender.OnYes);
        interactTexts = message;
        currentTextIndex = 0;

        interactText.SetText(interactTexts[currentTextIndex]);

        interactText.transform.parent.gameObject.SetActive(true);
        prompt.SetActive(true);

        PlayerInfo.Instance.ChangeInputMap("UI");
    }
    /// <summary>
    /// Ends the current prompted object interaction
    /// </summary>
    public void ResetPromptedInteraction() {
        interactText.transform.parent.gameObject.SetActive(false);
        promptedInteractionYesButton.onClick.RemoveAllListeners();
        prompt.SetActive(false);
        PlayerInfo.Instance.ChangeInputMap("Player");
    }
}