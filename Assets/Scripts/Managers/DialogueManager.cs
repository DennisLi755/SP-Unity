using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour {

    private DialogueManager instance;
    public DialogueManager Instance => instance;

    private DialogueRunner dialogueRunner;
    private GameObject dialogueCanvas;

    [SerializeField]
    private Color activeSpeakerColor;
    [SerializeField]
    private Color inActiveSpeakerColor;

    [SerializeField]
    private CharacterTalkPortraits[] charactersList;
    private Dictionary<string, TalkPortrait[]> characters = new Dictionary<string, TalkPortrait[]>();
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
    private bool lastActiveSpeaker = false;

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
        leftCharacterKey = null;
        rightCharacterImage = dialogueCanvas.transform.GetChild(1).GetComponent<Image>();
        rightCharacterKey = null;

        foreach (CharacterTalkPortraits cbt in charactersList) {
            characters.Add(string.Join(", ", cbt.aliases), cbt.portraits);
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
        if (leftCharacterKey.Contains(characterName)) {
            lastActiveSpeaker = false;
            leftCharacterImage.color = activeSpeakerColor;
            rightCharacterImage.color = inActiveSpeakerColor;
        }
        //the character has already spoken and is the right character
        else if (rightCharacterKey.Contains(characterName)) {
            lastActiveSpeaker = true;
            rightCharacterImage.color = activeSpeakerColor;
            leftCharacterImage.color = inActiveSpeakerColor;
        }
        //the character has not already spoken so check what the last character to speak was
        else {
            //right was the last to talk to replace left image with new character
            if (lastActiveSpeaker) {
                foreach (string aliases in characters.Keys) {
                    if (aliases.Contains(characterName)) {
                        leftCharacterKey = aliases;
                    }
                }
                leftCharacterImage.sprite = characters[leftCharacterKey][0].expression;
                leftCharacterImage.color = activeSpeakerColor;
                rightCharacterImage.color = inActiveSpeakerColor;
            }
            //left was the last to talk so replace right image with new character
            else {
                foreach (string aliases in characters.Keys) {
                    if (aliases.Contains(characterName)) {
                        rightCharacterKey = aliases;
                    }
                }
                rightCharacterImage.sprite = characters[leftCharacterKey][0].expression;
                rightCharacterImage.color = activeSpeakerColor;
                leftCharacterImage.color = inActiveSpeakerColor;
            }
            lastActiveSpeaker = !lastActiveSpeaker;
        }
    }

    public void UpdateExpression(string characterName, string expression) {

    }
}