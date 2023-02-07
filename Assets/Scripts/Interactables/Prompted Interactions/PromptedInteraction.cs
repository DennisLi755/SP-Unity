using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptedInteraction : TextboxInteraction
{
    [SerializeField]
    private List<string> afterYesMessage;
    public List<string> AfterYesMessage {get => afterYesMessage;}
    /// <summary>
    /// Base OnInteract() for all Prompted Interactions
    /// </summary>
    public override void OnInteract() {
        //show prompted interaction
        UIManager.Instance.ActivatePromptedInteraction(this, base.Message);
    }
    /// <summary>
    /// Base OnYes() for all Prompted Interactions
    /// </summary>
    public virtual void OnYes() {
        UIManager.Instance.ResetPromptedInteraction();
    }
}
