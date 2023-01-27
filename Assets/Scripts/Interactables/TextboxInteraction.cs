using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxInteraction : InteractableObject
{
    private Vector3 spawn;
    //text box sprite
    public List<string> message;
    public FacingDirection direction;
    private bool isActivated;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        spawn = transform.position;
        isActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void OnInteract()
    {
        if (PlayerInfo.Instance.PlayerControl.FacingDirection == direction)
        Debug.Log("Textbox interaction triggered");
    }
}
