using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class GuideInteract : PromptedInteraction {
    List<string> beforeText = new List<string>() { "Go pick up your sword, then we can fight." };

    List<string> afterText = new List<string>() { "Do you want to fight? " };

    public override void OnInteract() {
        if (PlayerInfo.Instance.AttackUnlocked) {
            UIManager.Instance.ActivatePromptedInteraction(this, afterText);
        }
        else {
            UIManager.Instance.ActivateInteractText(beforeText);
        }
    }
}