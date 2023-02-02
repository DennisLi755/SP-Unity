using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RailEnemy), true)]
[CanEditMultipleObjects]
public class RailEnemyEditor : StaticEnemyEditor {
    SerializedProperty nodes;
    SerializedProperty speed;

    private new void OnEnable() {
        base.OnEnable();
        nodes = serializedObject.FindProperty("nodes");
        speed = serializedObject.FindProperty("speed");
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.PropertyField(nodes);
        EditorGUILayout.PropertyField(speed);
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }
}
