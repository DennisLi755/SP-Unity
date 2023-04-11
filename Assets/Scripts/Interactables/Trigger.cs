using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Trigger : MonoBehaviour {

    [SerializeField]
    protected Bounds triggerBounds;
    protected Vector3 TriggerCenter => triggerBounds.center + transform.position;
    protected LayerMask playerLayer;
    protected bool triggered;
    protected System.Action onTrigger;

#if UNITY_EDITOR
    /// <summary>
    /// Draws debug info to the screen
    /// </summary>
    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(TriggerCenter, triggerBounds.size);
    }
#endif

    void Start() {
        playerLayer = LayerMask.GetMask("Player");
    }

    void Update() {
        RaycastHit2D hit = Physics2D.BoxCast(TriggerCenter, triggerBounds.size, 0, Vector2.zero, 0, playerLayer);
        if (hit && !triggered) {
            triggered = true;
            onTrigger();
        }
    }
}