using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem.UI;

public class UIManager : MonoBehaviour {
    private static UIManager instance;
    public static UIManager Instance {
        get {
            if (instance == null) {
                instance = new UIManager();
            }
            return instance;
        }
    }

    #region Object Interaction
    [Header("Object Interaction")]
    public TMP_Text interactText;
    List<string> interactTexts;
    private int currentTextIndex;
    [SerializeField]
    private Button promptedInteractionYesButton;
    [SerializeField]
    private Button textInteractionButton;
    [SerializeField]
    private EventSystem interactEventSystem;
    [SerializeField]
    private GameObject prompt;
    public GameObject Prompt {get => prompt; set {prompt = value;}}
    #endregion

    #region Health Bars
    [Header("Health Bars")]
    [SerializeField]
    private Image playerStandardHealthFill;
    [SerializeField]
    private GameObject playerStandardHealthBar;
    [SerializeField]
    private Image playerPentagonHealthFill;
    [SerializeField]
    private GameObject playerPentagonHealthBar;
    [SerializeField]
    private GameObject bossHealthBar;
    private Image bossHealthFill;
    private Image bossHealthBorder;
    #endregion

    #region Menu
    [Header("Menu")]
    [SerializeField]
    GameObject menu;
    [SerializeField]
    GameObject defaultSubMenu;
    GameObject currentSubMenu;
    [SerializeField]
    EventSystem menuEventSystem;
    GameObject lastSelectedObject;
    #endregion

    [Header("Misc.")]
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

    private void Start() {
        bossHealthFill = bossHealthBar.transform.GetChild(0).GetComponent<Image>();
        bossHealthBorder = bossHealthBar.transform.GetChild(1).GetComponent<Image>();

        currentSubMenu = defaultSubMenu;
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
        interactEventSystem.firstSelectedGameObject = textInteractionButton.gameObject;
        interactEventSystem.SetSelectedGameObject(textInteractionButton.gameObject);
        
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
        //disable the default interact text continue button and update the even system
        textInteractionButton.gameObject.SetActive(false);
        interactEventSystem.firstSelectedGameObject = promptedInteractionYesButton.gameObject;
        interactEventSystem.SetSelectedGameObject(promptedInteractionYesButton.gameObject);
        //update the the yes button with the yes command from the interactable object
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

    /// <summary>
    /// Updates the player's health bar fill to reflect their current health percent
    /// </summary>
    /// <param name="healthPercent"></param>
    public void UpdatePlayerHealth(float healthPercent) {
        playerStandardHealthFill.fillAmount = healthPercent;
        playerPentagonHealthFill.fillAmount = healthPercent;
    }

    /// <summary>
    /// Enables or disables the player health bar based on the given bool
    /// </summary>
    /// <param name="isEnabled"></param>
    public void EnablePlayerHealthBar(bool isEnabled) {
        playerStandardHealthBar.SetActive(isEnabled);
        playerPentagonHealthBar.SetActive(isEnabled);
    }

    /// <summary>
    /// Updates the boss health bar to reflect their current health percent
    /// </summary>
    /// <param name="healthPercent"></param>
    public void UpdateBossHealthBar(float healthPercent) {
        bossHealthFill.fillAmount = healthPercent;
    }

    /// <summary>
    /// Sets the border sprite used for the boss health bar
    /// </summary>
    /// <param name="asset"></param>
    public void SetBossHealthBarBorder(Sprite asset) {
        bossHealthBorder.sprite = asset;
    }

    /// <summary>
    /// Enables or disables the boss health bar based on the given bool
    /// </summary>
    /// <param name="isEnabled"></param>
    public void EnableBossHealthBar(bool isEnabled) {
        bossHealthBar.SetActive(isEnabled);
    }

    public void ToggleMenu() {
        menu.SetActive(!menu.activeInHierarchy);
        if (menu.activeInHierarchy) {
            menuEventSystem.SetSelectedGameObject(defaultSubMenu.transform.parent.gameObject);
            PlayerInfo.Instance.ChangeInputMap("UI");
        }
        else {
            PlayerInfo.Instance.ChangeInputMap("Player");
        }
    }

    public void EnableSubMenu(GameObject subMenu) {
        currentSubMenu.SetActive(false);
        currentSubMenu = subMenu;
        currentSubMenu.SetActive(true);
    }

    public void FocusSubMenu() {
        menuEventSystem.SetSelectedGameObject(currentSubMenu.transform.GetChild(2).gameObject);
    }

    public void UnFocusSubMenu() {
        menuEventSystem.SetSelectedGameObject(currentSubMenu.transform.parent.gameObject);
    }

    public void SetSelected(GameObject newSelected) {
        menuEventSystem.SetSelectedGameObject(newSelected);
    }

    public void SetLastSelected(GameObject sender) {
        lastSelectedObject = sender;
    }

    public void ReturnToLastSelected() {
        SetSelected(lastSelectedObject);
    }

    public void TryEquipSkill(int skillID) {
        SkillEquipStatus result = PlayerInfo.Instance.PlayerControl.EquipSkill(skillID, lastSelectedObject.name[^1] - '0');
        switch (result) {
            case SkillEquipStatus.Equipped:
                lastSelectedObject.GetComponentInChildren<TMP_Text>().text = skillID.ToString();
                ReturnToLastSelected();
                break;
            case SkillEquipStatus.NotUnlocked:
                //indicate that skill is not unlocked
                break;
            case SkillEquipStatus.Unequipped:
                lastSelectedObject.GetComponentInChildren<TMP_Text>().text = "";
                ReturnToLastSelected();
                break;
        }
    }
}