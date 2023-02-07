using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomLineView), true)]
public class LineViewEditor : UnityEditor.Editor {
    private SerializedProperty canvasGroupProperty;
    private SerializedProperty useFadeEffectProperty;
    private SerializedProperty fadeInTimeProperty;
    private SerializedProperty fadeOutTimeProperty;
    private SerializedProperty lineTextProperty;
    private SerializedProperty showCharacterNamePropertyInLineViewProperty;
    private SerializedProperty characterNameTextProperty;
    private SerializedProperty useTypewriterEffectProperty;
    private SerializedProperty onCharacterTypedProperty;
    private SerializedProperty typewriterEffectSpeedProperty;

    private SerializedProperty continueButtonProperty;

    private SerializedProperty autoAdvanceDialogueProperty;
    private SerializedProperty holdDelayProperty;

    public void OnEnable() {

        canvasGroupProperty = serializedObject.FindProperty("canvasGroup");
        useFadeEffectProperty = serializedObject.FindProperty("useFadeEffect");
        fadeInTimeProperty = serializedObject.FindProperty("fadeInTime");
        fadeOutTimeProperty = serializedObject.FindProperty("fadeOutTime");
        lineTextProperty = serializedObject.FindProperty("lineText");
        showCharacterNamePropertyInLineViewProperty = serializedObject.FindProperty("showCharacterNameInLineView");
        characterNameTextProperty = serializedObject.FindProperty("characterNameText");
        useTypewriterEffectProperty = serializedObject.FindProperty("useTypewriterEffect");
        onCharacterTypedProperty = serializedObject.FindProperty("onCharacterTyped");
        typewriterEffectSpeedProperty = serializedObject.FindProperty("typewriterEffectSpeed");
        continueButtonProperty = serializedObject.FindProperty("continueButton");
        autoAdvanceDialogueProperty = serializedObject.FindProperty("autoAdvance");
        holdDelayProperty = serializedObject.FindProperty("holdTime");
    }

    public override void OnInspectorGUI() {
        var baseProperties = new[] {
                canvasGroupProperty,

                lineTextProperty,

                autoAdvanceDialogueProperty,
            };
        foreach (var prop in baseProperties) {
            EditorGUILayout.PropertyField(prop);
        }

        if (autoAdvanceDialogueProperty.boolValue) {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(holdDelayProperty);
            EditorGUI.indentLevel -= 1;
        }

        EditorGUILayout.PropertyField(useFadeEffectProperty);

        if (useFadeEffectProperty.boolValue) {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(fadeInTimeProperty);
            EditorGUILayout.PropertyField(fadeOutTimeProperty);
            EditorGUI.indentLevel -= 1;
        }


        EditorGUILayout.PropertyField(useTypewriterEffectProperty);

        if (useTypewriterEffectProperty.boolValue) {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(onCharacterTypedProperty);
            EditorGUILayout.PropertyField(typewriterEffectSpeedProperty);
            EditorGUI.indentLevel -= 1;
        }

        EditorGUILayout.PropertyField(characterNameTextProperty);

        if (characterNameTextProperty.objectReferenceValue == null) {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(showCharacterNamePropertyInLineViewProperty);
            EditorGUI.indentLevel -= 1;
        }

        EditorGUILayout.PropertyField(continueButtonProperty);

        serializedObject.ApplyModifiedProperties();

    }
}