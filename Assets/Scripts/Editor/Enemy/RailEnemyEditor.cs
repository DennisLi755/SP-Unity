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

    protected void OnSceneGUI() {
        RailEnemy t = (RailEnemy)target;

        for (int i = 0; i < nodes.arraySize; i++) {
            EditorGUI.BeginChangeCheck();
            Vector3 oldPos = nodes.GetArrayElementAtIndex(i).vector3Value + t.transform.position;
            Vector3 newPos = Handles.DoPositionHandle(oldPos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck()) {
                t.nodes[i] = newPos - t.transform.position;
            }
        }

        if (Event.current.rawType == EventType.Used)
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
}