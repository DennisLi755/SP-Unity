using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractableObject : MonoBehaviour
{
    private BoxCollider2D trigger;
    public LayerMask playerLayer;
    bool isActive;
    // Start is called before the first frame update
    protected void Start()
    {
        trigger = GetComponent<BoxCollider2D>();
        isActive = false;
    }

    // Update is called once per frame
    protected void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(trigger.bounds.center, trigger.bounds.size, 0, Vector2.zero, 0, playerLayer);
        if (hit && !isActive) {
            isActive = true;
            //display prompt? depends on if we do it
            PlayerInfo.Instance.EnterInteractable(this);
        } else if (!hit && isActive) {
            isActive = false;
            //display prompt?
            PlayerInfo.Instance.ExitInteractable(this);
        }
    }

    public virtual void OnInteract() {}
}
