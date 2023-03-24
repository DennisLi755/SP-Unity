using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeaveHouse : EnterText
{
    public override void OnInteract() {
        if (!GameManager.Instance.GuideInteract) {
            UIManager.Instance.ActivateInteractText(new List<string>{"You should check out the body in the room before moving on."});
        } else {
            UIManager.Instance.endInteractionEvent.AddListener(EndInteraction);
            UIManager.Instance.FadeToBlack();
            StartCoroutine(WaitForFade());

            IEnumerator WaitForFade() {
                yield return new WaitForSeconds(1f);
                GameObject.Find("Level 1").GetComponent<Room>().MovePlayerHere();
                GameObject.Find("Level 1").GetComponent<Room>().MoveCameraHere();
                base.OnInteract();
            }
        }
    }

    void EndInteraction() {
        UIManager.Instance.FadeFromBlack();
        UIManager.Instance.endInteractionEvent.RemoveAllListeners();
    }
}
