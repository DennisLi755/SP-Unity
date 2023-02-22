using UnityEditor;

[CustomEditor(typeof(GuideAd))]
[CanEditMultipleObjects]
public class GuideAdEditor : RailEnemyEditor {
    private SerializedProperty afterImage;

    private new void OnEnable() {
        base.OnEnable();
        afterImage = serializedObject.FindProperty("afterImage");
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.PropertyField(afterImage);

        base.OnInspectorGUI();
    }
}