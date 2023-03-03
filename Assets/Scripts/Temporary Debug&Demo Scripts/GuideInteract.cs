using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class GuideInteract : PromptedInteraction {
    List<string> beforeText = new List<string>() { "Go pick up your sword, then we can fight." };

    List<string> afterText = new List<string>() { "Do you want to fight? " };

    public GameObject GuideBoss;
    public GameObject BossRoom;

    public override void OnInteract() {
        if (PlayerInfo.Instance.AttackUnlocked) {
            UIManager.Instance.ActivatePromptedInteraction(this, afterText);
        }
        else {
            UIManager.Instance.ActivateInteractText(beforeText);
        }
    }

    public override void OnYes() {
        StartCoroutine(BossStart());

        base.OnYes();
    }

    IEnumerator BossStart() {
        UIManager.Instance.FadeToBlack();
        yield return new WaitForSeconds(0.5f);
        BossRoom.GetComponent<Room>().MoveCameraHere();
        PlayerInfo.Instance.transform.position = new Vector3(97.0f, -80.75f, 0.0f);

        UIManager.Instance.FadeFromBlack();
        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(0.5f);

        Instantiate(GuideBoss, new Vector3(96.4f, -77.33f, 0.0f), Quaternion.identity);
    }
}