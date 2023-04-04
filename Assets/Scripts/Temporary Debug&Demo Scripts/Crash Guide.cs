using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CrashGuide : InteractableObject {
    private Animator anim;

    new void Start() {
        base.Start();
        anim = GetComponent<Animator>();
    }

    public override void OnInteract() {
        //StartCoroutine(StandUp());
        if (GameManager.Instance.GetProgressionFlag("Not Ready")) {
            DialogueManager.Instance.StartDialogue("Are_You_Ready?");
        } else {
            DialogueManager.Instance.StartDialogue("Meet_Guide");
        }
    }

    /// <summary>
    /// Plays Guide's stand up animation and then starts dialogue with her
    /// </summary>
    /// <returns></returns>
    private IEnumerator StandUp() {
        anim.SetTrigger("Stand Up");
        PlayerInfo.Instance.PlayerControl.Freeze();
        yield return new WaitForSeconds(2.0f);
        //start dialogue
        DialogueManager.Instance.StartDialogue("Meet_Guide");
    }
}