using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractableObject : MonoBehaviour {
    protected BoxCollider2D trigger;
    [SerializeField]
    protected LayerMask playerLayer;
    [SerializeField]
    protected FacingDirection direction;
    protected bool isActive;

#if UNITY_EDITOR
    /// <summary>
    /// Draws debug info to the screen
    /// </summary>
    private void OnDrawGizmos() {
        if (!EditorApplication.isPlaying && trigger == null) {
            trigger = GetComponent<BoxCollider2D>();
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(trigger.bounds.center, trigger.bounds.size);
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    protected void Start() {
        trigger = GetComponent<BoxCollider2D>();
        isActive = false;
    }
    /// <summary>
    /// Detects whether or not the player interacts with the object
    /// </summary>
    protected virtual void Update() {
        RaycastHit2D hit = Physics2D.BoxCast(trigger.bounds.center, trigger.bounds.size, 0, Vector2.zero, 0, playerLayer);
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
}