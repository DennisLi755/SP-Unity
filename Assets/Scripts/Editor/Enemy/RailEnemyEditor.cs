using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RailEnemy))]
[CanEditMultipleObjects]
public class RailEnemyEditor : StaticEnemyEditor {
    protected SerializedProperty nodes;
    protected SerializedProperty speed;
    protected SerializedProperty moveBulletsWithEnemy;

    protected new void OnEnable() {
        base.OnEnable();
        nodes = serializedObject.FindProperty("nodes");
        speed = serializedObject.FindProperty("speed");
        moveBulletsWithEnemy = serializedObject.FindProperty("moveBulletsWithEnemy");
    }

    public override void OnInspectorGUI() {
        EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
        EditorGUILayout.PropertyField(nodes);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(speed);
        EditorGUILayout.PropertyField(moveBulletsWithEnemy);
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }
}