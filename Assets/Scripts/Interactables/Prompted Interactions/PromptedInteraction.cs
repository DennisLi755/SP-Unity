using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptedInteraction : TextboxInteraction
{
    [SerializeField]
    private List<string> afterYesMessage;
    public List<string> AfterYesMessage {get => afterYesMessage;}
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    public override void OnInteract() {
        //show prompted interaction
        UIManager.Instance.ActivatePromptedInteraction(this, base.Message);
    }

    public virtual void OnYes() {
        UIManager.Instance.ResetPromptedInteraction();
    }
}
