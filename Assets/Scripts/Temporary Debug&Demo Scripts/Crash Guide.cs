using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CrashGuide : InteractableObject {
    private Animator anim;
    private bool hasInteracted = false;

    public void Start() {
        base.Start();
        anim = GetComponent<Animator>();
    }

    public override void OnInteract() {
        if (hasInteracted) {
            return;
        }
        hasInteracted = true;
        StartCoroutine(StandUp());
    }

    private IEnumerator StandUp() {
        anim.SetTrigger("Stand Up");
        PlayerInfo.Instance.PlayerControl.Freeze();
        yield return new WaitForSeconds(2.0f);
        //start dialogue
        DialogueManager.Instance.StartDialogue("Meet_Guide");
    }
}