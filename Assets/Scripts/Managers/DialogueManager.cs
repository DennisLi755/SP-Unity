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

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(transform.parent);
        }
        else {
            Destroy(this);
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

        foreach (CharacterTalkPortraits cbt in charactersList) {
            Dictionary<string, Sprite> expressions = new Dictionary<string, Sprite>();
            foreach (TalkPortrait dp in cbt.portraits) {
                expressions.Add(dp.name, dp.expression);
            }
            characters.Add(string.Join(", ", cbt.aliases), expressions);
        }
    }

    void Update() {

    }

    public void DisableDialogueCanvas() {
        EnableDialogueCanvas(false);
    }

    public void EnableDialogueCanvas() {
        EnableDialogueCanvas(true);
    }

    private void EnableDialogueCanvas(bool value) {
        dialogueCanvas.SetActive(value);
    }

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

    public void UpdateExpression(string expression) {
        //updating the left character expression
        try {
            if (!lastActiveSpeaker) {
                leftCharacterImage.sprite = characters[leftCharacterKey][expression];
            }
            else {
                rightCharacterImage.sprite = characters[rightCharacterKey][expression];
            }
        } catch (KeyNotFoundException ex) {
            Debug.LogError($"Error: there is no expression: ${expression} for character with the following aliases: {(lastActiveSpeaker? rightCharacterKey : leftCharacterKey)}");
        }
    }

    public void StartDialogue(YarnProject project, string startNode) {
        leftCharacterKey = null;
        rightCharacterKey = null;
        dialogueRunner.SetProject(project);
        dialogueRunner.startNode = startNode;
        PlayerInfo.Instance.ChangeInputMap("UI");
    }

    public void StartDialogue() {

    }

    public void ToggleAutoAdvance() {

    }
}