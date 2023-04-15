using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    [SerializeField]
    protected Bounds triggerBounds;
    public Vector3 Trigger => triggerBounds.center + transform.position;
    protected LayerMask playerLayer;
    [SerializeField]
    protected FacingDirection direction;
    protected bool isActive;

#if UNITY_EDITOR
    /// <summary>
    /// Draws debug info to the screen
    /// </summary>
    protected void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(Trigger, triggerBounds.size);
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    protected void Start() {
        playerLayer = LayerMask.GetMask("Player");
        isActive = false;
    }
    /// <summary>
    /// Detects whether or not the player interacts with the object
    /// </summary>
    protected virtual void Update() {
        RaycastHit2D hit = Physics2D.BoxCast(Trigger, triggerBounds.size, 0, Vector2.zero, 0, playerLayer);
        if (hit && !isActive) {
            isActive = true;
            //display prompt? depends on if we do it
            PlayerInfo.Instance.EnterInteractable(this);
        }
        else if (!hit && isActive) {
            isActive = false;
            //display prompt?
            PlayerInfo.Instance.ExitInteractable(this);
        }
    }
    /// <summary>
    /// Base OnInteract() for InteractableObject
    /// </summary>
    public virtual void OnInteract() { }

    /// <summary>
    /// Checks if the player is facing the correct way to interact with the object or they object is set to omni-directional
    /// </summary>
    /// <returns></returns>
    public bool ValidatePlayerDirection() {
        return PlayerInfo.Instance.PlayerControl.FacingDirection == direction || direction == FacingDirection.Omni;
    }
}