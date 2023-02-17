using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RailEnemy : StaticEnemy {

    [SerializeField]
    protected bool moveBulletsWithEnemy;
    [SerializeField]
    private List<Vector3> nodes;
    private int currentNodeIndex = 0;
    private Vector3 Target => nodes[(currentNodeIndex + 1) % nodes.Count];
    [SerializeField]
    private float speed;

#if UNITY_EDITOR
    private new void OnDrawGizmos() {
        base.OnDrawGizmos();
        Handles.color = Color.grey;
        Gizmos.color = Color.grey;
        if (EditorApplication.isPlaying) {
            for (int i = 0; i < nodes.Count; i++) {
                Handles.DrawSolidDisc(nodes[i], Vector3.back, 0.2f); ;
                if (nodes.Count > 1) {
                    Gizmos.DrawLine(nodes[i], (i < nodes.Count - 1) ? nodes[i + 1] : nodes[0]);
                }
            }
        }
        else {
            for (int i = 0; i < nodes.Count; i++) {
                UnityEditor.Handles.DrawSolidDisc(nodes[i] + transform.position, Vector3.back, 0.2f);
                if (nodes.Count > 1) {
                    Gizmos.DrawLine(nodes[i] + transform.position, ((i < nodes.Count - 1) ? nodes[i + 1] : nodes[0]) + transform.position);
                }
            }
        }
    }
#endif

    private new void Start() {
        base.Start();

        //transcribe nodes to world position
        for (int i = 0; i < nodes.Count; i++) {
            nodes[i] += transform.position;
        }
    }

    private new void Update() {
        base.Update();
        
        if (transform.position == Target) {
            currentNodeIndex = (currentNodeIndex + 1) % nodes.Count;
        }
    }

    private void FixedUpdate() {
        transform.position = Vector3.MoveTowards(transform.position, Target, speed * Time.fixedDeltaTime);
    }

    public override void ShootPatternBullet(GameObject pattern) {
        if (moveBulletsWithEnemy) {
            Instantiate(pattern, transform);
        }
        else {
            base.ShootPatternBullet(pattern);
        }
    }

    [ContextMenu("Move to next node")]
    public void MoveToNextNode() {
        currentNodeIndex = (currentNodeIndex + 1) % nodes.Count;
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