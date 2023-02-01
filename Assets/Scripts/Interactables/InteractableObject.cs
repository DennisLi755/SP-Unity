using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractableObject : MonoBehaviour {
    private BoxCollider2D trigger;
    [SerializeField]
    private LayerMask playerLayer;
    private bool isActive;

    private void OnDrawGizmos() {
        if (!EditorApplication.isPlaying && trigger == null) {
            trigger = GetComponent<BoxCollider2D>();
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(trigger.bounds.center, trigger.bounds.size);
    }

    protected void Start() {
        trigger = GetComponent<BoxCollider2D>();
        isActive = false;
    }

    protected void Update() {
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

    public virtual void OnInteract() { }
}