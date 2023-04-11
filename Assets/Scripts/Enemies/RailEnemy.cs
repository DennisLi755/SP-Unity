using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RailEnemy : StaticEnemy {

    [SerializeField]
    protected bool moveBulletsWithEnemy;
    [SerializeField]
    public List<Vector3> nodes;
    protected int currentNodeIndex = 0;
    protected Vector3 Target {
        get {
            if (nodes.Count == 0) {
                return transform.position ;
            }
            return nodes[(currentNodeIndex + 1) % nodes.Count];
        }
    }
    [SerializeField]
    protected float speed;

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
                Handles.DrawSolidDisc(nodes[i] + transform.position, Vector3.back, 0.2f);
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
        
        if (nodes.Count > 1 && transform.position == Target) {
            currentNodeIndex = (currentNodeIndex + 1) % nodes.Count;
        }
    }

    private void FixedUpdate() {
        if (!(transform.position == Target)) {
            transform.position = Vector3.MoveTowards(transform.position, Target, speed * Time.fixedDeltaTime);
        }
    }

    /// <inheritdoc/>
    public override void ShootPatternBullet(GameObject pattern) {
        if (moveBulletsWithEnemy) {
            Instantiate(pattern, transform);
        }
        else {
            base.ShootPatternBullet(pattern);
        }
    }

    /// <summary>
    /// Adds a node to the enemy's node list
    /// </summary>
    /// <param name="newNode"></param>
    public void AddNode(Vector3 newNode) {
        nodes.Add(newNode);
    }
}