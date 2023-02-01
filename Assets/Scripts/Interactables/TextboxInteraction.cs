using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxInteraction : InteractableObject
{
    private Vector3 spawn;
    //text box sprite
    public List<string> message;
    public FacingDirection direction;
    public bool isChild = false;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        spawn = transform.position;
        if (isChild) {
            message = transform.parent.gameObject.GetComponent<TextboxInteraction>().message;
        }
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void OnInteract()
    {
        if (PlayerInfo.Instance.PlayerControl.FacingDirection == direction) {
            //Debug.Log("Textbox interaction triggered");
            UIManager.Instance.ActivateInteractText(message);
        }
    }
}
