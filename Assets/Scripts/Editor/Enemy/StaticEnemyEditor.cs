using UnityEditor;
using UnityEngine;
using Unity.Collections;

[CustomEditor(typeof(StaticEnemy))]
[CanEditMultipleObjects]
public class StaticEnemyEditor : Editor {
    protected SerializedProperty useTargetingCircle;
    protected SerializedProperty attackCycle;
    protected SerializedProperty playerLayer;

    protected GameObject gb;

    protected void OnEnable() {
        useTargetingCircle = serializedObject.FindProperty("useTargetingCircle");
        attackCycle = serializedObject.FindProperty("attackCycle");
        playerLayer = serializedObject.FindProperty("playerLayer");

        gb = ((StaticEnemy)target).gameObject;
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.PropertyField(attackCycle);
        EditorGUILayout.PropertyField(useTargetingCircle);
        if (useTargetingCircle.boolValue) {
            EditorGUILayout.PropertyField(playerLayer);
            if (gb.GetComponent<CircleCollider2D>() == null) {
                gb.AddComponent<CircleCollider2D>();
            }
        }
        else if (gb.GetComponent<CircleCollider2D>() != null) {
            DestroyImmediate(gb.GetComponent<CircleCollider2D>());
        }

        serializedObject.ApplyModifiedProperties();
    }
}