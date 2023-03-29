using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour {

    private static DialogueManager instance;
    public static DialogueManager Instance => instance;

    private DialogueRunner dialogueRunner;
    private GameObject dialogueCanvas;

    [SerializeField]
    private Color activeSpeakerColor;
    [SerializeField]
    private Color inActiveSpeakerColor;
    private Color disabledColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    #region Talking Character Info
    [SerializeField]
    private CharacterTalkPortraits[] charactersList;
    private Dictionary<string, Dictionary<string, Sprite>> characters = new Dictionary<string, Dictionary<string, Sprite>>();
    private Image leftCharacterImage;
    private string leftCharacterKey;
    private Image rightCharacterImage;
    private string rightCharacterKey;
    /// <summary>
    /// Tracks which character was the last to speak in order to know which character to replace when a new character speaks
    /// </summary>
    /// <remarks>
    /// False means the left character spoke last and true means the right character spoke last
    /// </remarks>
    private bool lastActiveSpeaker = true;
    #endregion

    private void Awake() {
        if (instance == null) {
            instance = this;
            //DontDestroyOnLoad(transform.parent.gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        dialogueRunner = GetComponent<DialogueRunner>();
        dialogueCanvas = transform.GetChild(0).gameObject;

        leftCharacterImage = dialogueCanvas.transform.GetChild(0).GetComponent<Image>();
        leftCharacterImage.color = disabledColor;
        leftCharacterKey = null;
        rightCharacterImage = dialogueCanvas.transform.GetChild(1).GetComponent<Image>();
        rightCharacterImage.color = disabledColor;
        rightCharacterKey = null;

        //populate the talk portrait dictionary with expressions for each character by gonig through the inspector list
        foreach (CharacterTalkPortraits cbt in charactersList) {
            Dictionary<string, Sprite> expressions = new Dictionary<string, Sprite>();
            foreach (TalkPortrait dp in cbt.portraits) {
                expressions.Add(dp.name, dp.expression);
            }
            //consolodate a character's alias to use as the key for their expressions
            characters.Add(string.Join(", ", cbt.aliases), expressions);
        }
    }

    /// <summary>
    /// Enables the dialogue canvas
    /// </summary>
    public void DisableDialogueCanvas() {
        EnableDialogueCanvas(false);
        PlayerInfo.Instance.PlayerControl.UnFreeze();
        PlayerInfo.Instance.ChangeInputMap("Player");
    }

    /// <summary>
    /// Disables the dialogue canvas
    /// </summary>
    public void EnableDialogueCanvas() {
        EnableDialogueCanvas(true);
    }

    /// <summary>
    /// Enables or disables the dialogue canvas based on given bool
    /// </summary>
    /// <param name="isEnabled"></param>
    public void EnableDialogueCanvas(bool isEnabled) {
        dialogueCanvas.SetActive(isEnabled);
    }

    /// <summary>
    /// Changes the color of the coresponding character's sprite. If the character is not one of the last two characters to talk,
    /// their sprite will replace the character who was not the most recent to talk
    /// </summary>
    /// <param name="characterName"></param>
    public void UpdateActiveSpeaker(string characterName) {
        //the character has already spoken and is the left character
        if (leftCharacterKey != null && leftCharacterKey.Contains(characterName)) {
            lastActiveSpeaker = false;
            leftCharacterImage.color = activeSpeakerColor;
            if (rightCharacterKey != null) {
                rightCharacterImage.color = inActiveSpeakerColor;
            }
        }
        //the character has already spoken and is the right character
        else if (rightCharacterKey != null && rightCharacterKey.Contains(characterName)) {
            lastActiveSpeaker = true;
            rightCharacterImage.color = activeSpeakerColor;
            if (leftCharacterKey != null) {
                leftCharacterImage.color = inActiveSpeakerColor;
            }
        }
        //the character has not already spoken so check what the last character to speak was
        else {
            //right was the last to talk to replace left image with new character
            if (lastActiveSpeaker) {
                leftCharacterKey = null;
                foreach (string aliases in characters.Keys) {
                    if (aliases.Contains(characterName)) {
                        leftCharacterKey = aliases;
                        break;
                    }
                }
                //if there was no coresponding character found, throw error 
                if (leftCharacterKey == null) {
                    Debug.LogError($"Error: there is no character with a known alias of {characterName}");
                    leftCharacterImage.color = disabledColor;
                    return;
                }
                leftCharacterImage.sprite = characters[leftCharacterKey]["neutral"];
                leftCharacterImage.color = activeSpeakerColor;
                if (rightCharacterKey != null) {
                    rightCharacterImage.color = inActiveSpeakerColor;
                }
            }
            //left was the last to talk so replace right image with new character
            else {
                rightCharacterKey = null;
                foreach (string aliases in characters.Keys) {
                    if (aliases.Contains(characterName)) {
                        rightCharacterKey = aliases;
                        break;
                    }
                }
                if (rightCharacterKey == null) {
                    Debug.LogError($"Error: there is no character with a known alias of {characterName}");
                    rightCharacterImage.color = disabledColor;
                    return;
                }
                rightCharacterImage.sprite = characters[rightCharacterKey]["neutral"];
                rightCharacterImage.color = activeSpeakerColor;
                if (leftCharacterKey != null) {
                    leftCharacterImage.color = inActiveSpeakerColor;
                }
            }
            //regardless of who gets replaced, flip the last active speaker
            lastActiveSpeaker = !lastActiveSpeaker;
        }
    }

    /// <summary>
    /// Updates the currently speaking character's expression
    /// </summary>
    /// <param name="expression"></param>
    public void UpdateExpression(string expression) {
        //updating the left character expression
        try {
            if (!lastActiveSpeaker) {
                leftCharacterImage.sprite = characters[leftCharacterKey][expression];
            }
            else {
                rightCharacterImage.sprite = characters[rightCharacterKey][expression];
            }
        } catch (KeyNotFoundException) {
            Debug.LogError($"Error: there is no expression: ${expression} for character with the following aliases: {(lastActiveSpeaker? rightCharacterKey : leftCharacterKey)}");
        }
    }

    /// <summary>
    /// Starts the dialogue by setting up the DialogueRunner and enables the DialogueCanvas
    /// </summary>
    /// <param name="project"></param>
    /// <param name="startNode"></param>
    public void StartDialogue(YarnProject project, string startNode) {
        dialogueRunner.SetProject(project);
        StartDialogue(startNode);
    }

    public void StartDialogue(string startNode) {
        lastActiveSpeaker = true;
        leftCharacterKey = null;
        rightCharacterKey = null;
        dialogueCanvas.SetActive(true);
        dialogueRunner.StartDialogue(startNode);
        PlayerInfo.Instance.ChangeInputMap("UI");
    }

    /// <summary>
    /// Toggles auto advance dialogue on and off. Used when characters cut each other and the player cannot control it
    /// </summary>
    public void ToggleAutoAdvance() {

    }

    /// <summary>
    /// Toggles the character name plate on and off
    /// </summary>
    /// <param name="isEnabled"></param>
    public void ToggleCharacterNamePlate(bool isEnabled) {
        dialogueCanvas.transform.GetChild(3).gameObject.SetActive(isEnabled);
    }
}