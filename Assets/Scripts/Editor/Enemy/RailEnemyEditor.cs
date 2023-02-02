using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RailEnemy), true)]
[CanEditMultipleObjects]
public class RailEnemyEditor : StaticEnemyEditor {
    private SerializedProperty nodes;
    private SerializedProperty speed;
    private SerializedProperty moveBulletsWithEnemy;

    private new void OnEnable() {
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
