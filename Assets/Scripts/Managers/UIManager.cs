using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;
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
    ///Characters per second
    const float typewriterSpeed = 30.0f;
    private Coroutine typewriterRoutine;
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
    //player health
    [SerializeField]
    private Image playerStandardHealthFill;
    [SerializeField]
    private GameObject playerStandardHealthBar;
    //player mana
    [SerializeField]
    private Image playerManaFill;
    [SerializeField]
    private GameObject playerManaBar;

    //boss
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

    //Skills
    [SerializeField]
    GameObject[] skillSlots = new GameObject[2];
    [SerializeField]
    GameObject[] skillIcons = new GameObject[2];
    #endregion

    [Header("Misc.")]
    [SerializeField]
    Image fadeToBlackPanel;
    [SerializeField]
    Image fadeToWhitePanel;

    public System.Action EndInteractionEvent { get; set; }

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

        if (skillSlots[0] == null || skillSlots[1] == null) {
            Debug.LogWarning("UIManager's Skill Slot game objects are not setup correctly");
        }
    }

    #region Fade to Black/White
    /// <summary>
    /// Fades the screen to black
    /// </summary>
    [ContextMenu("FadeToBlack")]
    public void FadeToBlack() {
        Fade(true, "FadeToBlack");
    }

    /// <summary>
    /// Fades the screen from black
    /// </summary>
    [ContextMenu("Fade From Black")]
    public void FadeFromBlack() {
        Fade(false, "FadeFromBlack");
    }

    public void FadeToWhite() {
        Fade(true, "FadeToWhite");
    }

    public void FadeFromWhite() {
        Fade(false, "FadeFromWhite");
    }

    /// <summary>
    /// Controls whether the screen fades to or from black based on the paramter
    /// </summary>
    /// <param name="fade">true causes the screen to fade to black; false causes the screen to from from black</param>
    private void Fade(bool fade, string animName) {
        if (fade) {
            fadeToBlackPanel.GetComponent<Animation>().Play(animName);
        }
        else {
            fadeToBlackPanel.GetComponent<Animation>().Play(animName);
        }
    }

    public void CutFromBlack() {
        Color c = fadeToBlackPanel.GetComponent<Image>().color;
        c.a = 0;
        fadeToBlackPanel.GetComponent<Image>().color = c;
    }
    #endregion

    #region Object Interaction UI
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
        typewriterRoutine = StartCoroutine(TypewriterEffect(false));

        //disable the player's movement
        PlayerInfo.Instance.ChangeInputMap("UI");
    }

    IEnumerator TypewriterEffect(bool isPrompted) {
        //start everything invisible
        interactText.maxVisibleCharacters = 0;

        //wait 1 frame to let the text box update its content
        yield return null;

        int characterCount = interactText.textInfo.characterCount;

        float secondsPerCharacter = 1.0f / typewriterSpeed;

        //in order to account for frame drops, we use the time between frames to calculate how many characters to show
        float time = Time.deltaTime;

        while (interactText.maxVisibleCharacters < characterCount) {
            while (time >= secondsPerCharacter) {
                interactText.maxVisibleCharacters++;
                SoundManager.Instance.PlayUISoundEffect("Text Type");
                time -= secondsPerCharacter;
            }

            time += Time.deltaTime;
            yield return null;
        }

        interactText.maxVisibleCharacters = characterCount;
        typewriterRoutine = null;
        if (isPrompted) {
            prompt.SetActive(true);
        }
    }

    /// <summary>
    /// When the current messages finish either continue or end the interaction
    /// </summary>
    public void ContinueInteractText() {
        //not every character has been displayed, so interrupt the typewriter effect
        if (interactText.maxVisibleCharacters < interactText.textInfo.characterCount) {
            //StopCoroutine(typewriterRoutine);
            interactText.maxVisibleCharacters = interactText.textInfo.characterCount;
        }
        else {
            if (currentTextIndex == interactTexts.Count) {
                DeactivateInteractText();
            }
            else {
                interactText.SetText(interactTexts[currentTextIndex++]);
                typewriterRoutine = StartCoroutine(TypewriterEffect(false));
            }
        }
    }

    /// <summary>
    /// End the current object interaction
    /// </summary>
    public void DeactivateInteractText() {
        interactText.transform.parent.gameObject.SetActive(false);
        PlayerInfo.Instance.ChangeInputMap("Player");
        EndInteractionEvent?.Invoke();
        EndInteractionEvent = null;
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
        typewriterRoutine = StartCoroutine(TypewriterEffect(true));

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
    #endregion
    #region Combat UI
    /// <summary>
    /// Updates the player's health bar fill to reflect their current health percent
    /// </summary>
    /// <param name="healthPercent"></param>
    public void UpdatePlayerHealth(float healthPercent) {
        playerStandardHealthFill.fillAmount = healthPercent;
    }

    /// <summary>
    /// Enables or disables the player health bar based on the given bool
    /// </summary>
    /// <param name="isEnabled"></param>
    public void EnablePlayerHealthBar(bool isEnabled) {
        playerStandardHealthBar.SetActive(isEnabled);
    }

    public void UpdatePlayerMana(float manaPercent) {
        playerManaFill.fillAmount = manaPercent;
    }

    public void EnablePlayerManaBar(bool isEnabled) {
        playerManaBar.SetActive(isEnabled);
    }

    /// <summary>
    /// Updates the UI element representing a skil's cooldown
    /// </summary>
    /// <param name="skillSlot"></param>
    /// <param name="cooldownPercent"></param>
    public void UpdateSkillIconCooldown(int skillSlot, float cooldownPercent) {
        skillIcons[skillSlot].transform.GetChild(1).GetComponent<Image>().fillAmount = cooldownPercent;
    }

    /// <summary>
    /// Toggles the visibility of the player's currently equipped skill cooldown icons under their health
    /// </summary>
    /// <param name="isEnabled"></param>
    public void EnableSkillIcons(bool isEnabled) {
        skillIcons[0].transform.parent.gameObject.SetActive(isEnabled);
    }

    public void EnableDashCharges(bool isEnabled) {

    }

    public void EnableCombatUI(bool isEnabled) {
        EnablePlayerHealthBar(isEnabled);
        //EnablePlayerManaBar(isEnabled);
        EnableSkillIcons(isEnabled);
        EnableDashCharges(isEnabled);
    }

    #region Boss Healthbar
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
    #endregion
    #endregion
    #region Pause Menu
    /// <summary>
    /// Toggles the menu's visibility
    /// </summary>
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

    /// <summary>
    /// Switches the currently shown sub menu preview
    /// </summary>
    /// <param name="subMenu"></param>
    public void EnableSubMenu(GameObject subMenu) {
        currentSubMenu.SetActive(false);
        currentSubMenu = subMenu;
        currentSubMenu.SetActive(true);
    }

    /// <summary>
    /// Changes the player's navigation to enter a submenu
    /// </summary>
    public void FocusSubMenu() {
        menuEventSystem.SetSelectedGameObject(currentSubMenu.transform.GetChild(2).gameObject);
    }

    /// <summary>
    /// Changes the player's navigation back to the sub-menu selection tabs
    /// </summary>
    public void UnFocusSubMenu() {
        menuEventSystem.SetSelectedGameObject(currentSubMenu.transform.parent.gameObject);
    }

    /// <summary>
    /// Sets the selected UI gameobject in the event system
    /// </summary>
    /// <param name="newSelected"></param>
    public void SetSelected(GameObject newSelected) {
        menuEventSystem.SetSelectedGameObject(newSelected);
    }

    /// <summary>
    /// The last selected game object that will be re-focused when the player presses 'cancel'
    /// </summary>
    /// <param name="sender"></param>
    public void SetLastSelected(GameObject sender) {
        lastSelectedObject = sender;
    }

    /// <summary>
    /// Selects the last seleceted UI game object
    /// </summary>
    public void ReturnToLastSelected() {
        SetSelected(lastSelectedObject);
    }

    /// <summary>
    /// Tries to equip a skill
    /// </summary>
    /// <param name="skillID"></param>
    public void TryEquipSkill(int skillID) {
        int targetSlot = lastSelectedObject.name[^1] - '0';
        SkillEquipStatus result = PlayerInfo.Instance.PlayerControl.EquipSkill(skillID, targetSlot);
        switch (result) {
            case SkillEquipStatus.Equipped:
                skillSlots[targetSlot].GetComponentInChildren<TMP_Text>().text = skillID.ToString();

                skillIcons[targetSlot].SetActive(true);
                skillIcons[targetSlot].GetComponentInChildren<TMP_Text>().text = skillID.ToString();
                ReturnToLastSelected();
                break;
            case SkillEquipStatus.NotUnlocked:
                //indicate that skill is not unlocked
                break;
            case SkillEquipStatus.Unequipped:
                skillSlots[targetSlot].GetComponentInChildren<TMP_Text>().text = "";

                skillIcons[targetSlot].SetActive(false);
                skillIcons[targetSlot].GetComponentInChildren<TMP_Text>().text = "";
                ReturnToLastSelected();
                break;
            case SkillEquipStatus.Swapped:
                int otherSlot = Mathf.Abs(targetSlot - 1);
                skillSlots[targetSlot].GetComponentInChildren<TMP_Text>().text = skillID.ToString();
                skillSlots[otherSlot].GetComponentInChildren<TMP_Text>().text = "";

                //enable the new skill icon
                skillIcons[targetSlot].SetActive(true);
                skillIcons[targetSlot].GetComponentInChildren<TMP_Text>().text = skillID.ToString();
                //disable the old skill icon
                skillIcons[otherSlot].SetActive(false);
                skillIcons[otherSlot].GetComponentInChildren<TMP_Text>().text = "";
                break;
        }
    }
    #endregion
}