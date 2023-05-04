using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : Trigger {
    [SerializeField]
    private string keyboardText;
    [SerializeField]
    private string controllerText;
    [SerializeField]
    private string tutorialKey;
    protected override void OnTrigger() {
        PlayerInfo.Instance.EnableTutorialText(keyboardText, controllerText);
        PlayerInfo.Instance.Tutorials[tutorialKey] = false;
        base.OnTrigger();
    }
}
