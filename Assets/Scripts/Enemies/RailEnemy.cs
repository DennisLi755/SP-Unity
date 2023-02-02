using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailEnemy : StaticEnemy {

    [SerializeField]
    private Vector3[] nodes;
    private int currentNodeIndex = 0;
    private Vector3 Target => nodes[(currentNodeIndex + 1) % nodes.Length];
    [SerializeField]
    private float speed;

    private void OnDrawGizmos() {
        UnityEditor.Handles.color = Color.grey;
        Gizmos.color = Color.grey;
        for (int i = 0; i < nodes.Length; i++) {
            UnityEditor.Handles.DrawSolidDisc(nodes[i], Vector3.back, 0.2f); ;
            if (nodes.Length > 1) {
                Gizmos.DrawLine(nodes[i], (i < nodes.Length - 1) ? nodes[i + 1] : nodes[0]);
            }
        }
    }

    private new void Start() {
        base.Start();
    }

    private new void Update() {
        base.Update();
        
        if (transform.position == Target) {
            currentNodeIndex = (currentNodeIndex + 1) % nodes.Length;
        }
    }

    private void FixedUpdate() {
        transform.position = Vector3.MoveTowards(transform.position, Target, speed * Time.fixedDeltaTime);
    }

    [ContextMenu("Move to next node")]
    public void MoveToNextNode() {
        currentNodeIndex = (currentNodeIndex + 1) % nodes.Length;
        transform.position = nodes[currentNodeIndex];
        if (useTargetingCircle) {
            targetingCircle.enabled = false;
            targetingCircle.enabled = true;
        }
    }

    [ContextMenu("Reset to start node")]
    public void ResetToNode() {
        transform.position = nodes[currentNodeIndex];
    }
}