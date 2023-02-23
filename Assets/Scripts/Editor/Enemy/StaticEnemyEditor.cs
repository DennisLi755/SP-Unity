using UnityEditor;
using UnityEngine;
using Unity.Collections;

[CustomEditor(typeof(StaticEnemy))]
[CanEditMultipleObjects]
public class StaticEnemyEditor : Editor {
    protected SerializedProperty maxHealth;
    protected SerializedProperty useTargetingCircle;
    protected SerializedProperty attackCycle;
    protected SerializedProperty playerLayer;
    protected SerializedProperty targetingCircle;
    protected SerializedProperty healthDrop;
    protected SerializedProperty healthDropChance;

    protected void OnEnable() {
        maxHealth = serializedObject.FindProperty("maxHealth");
        useTargetingCircle = serializedObject.FindProperty("useTargetingCircle");
        attackCycle = serializedObject.FindProperty("attackCycle");
        playerLayer = serializedObject.FindProperty("playerLayer");
        targetingCircle = serializedObject.FindProperty("targetingCircle");
        healthDrop = serializedObject.FindProperty("healthDrop");
        healthDropChance = serializedObject.FindProperty("healthDropChance");
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.PropertyField(maxHealth);
        EditorGUILayout.PropertyField(attackCycle);
        EditorGUILayout.PropertyField(useTargetingCircle);
        if (useTargetingCircle.boolValue) {
            EditorGUILayout.PropertyField(playerLayer);
            EditorGUILayout.PropertyField(targetingCircle);
        }
        EditorGUILayout.PropertyField(healthDrop);
        EditorGUILayout.PropertyField(healthDropChance);

        serializedObject.ApplyModifiedProperties();
    }
}