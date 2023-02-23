using UnityEditor;
using UnityEngine;
using Unity.Collections;

[CustomEditor(typeof(StaticEnemy))]
[CanEditMultipleObjects]
public class StaticEnemyEditor : Editor {
    protected SerializedProperty startingHealth;
    protected SerializedProperty attackCycle;

    protected SerializedProperty useTargetingCircle;
    protected SerializedProperty playerLayer;
    protected SerializedProperty targetingCircle;

    protected SerializedProperty healthDrop;
    protected SerializedProperty healthDropChance;

    protected SerializedProperty hasAggroLoss;
    protected SerializedProperty aggroLossTime;

    protected void OnEnable() {
        startingHealth = serializedObject.FindProperty("startingHealth");
        attackCycle = serializedObject.FindProperty("attackCycle");

        useTargetingCircle = serializedObject.FindProperty("useTargetingCircle");
        playerLayer = serializedObject.FindProperty("playerLayer");
        targetingCircle = serializedObject.FindProperty("targetingCircle");

        healthDropChance = serializedObject.FindProperty("healthDropChance");
        healthDrop = serializedObject.FindProperty("healthDrop");

        hasAggroLoss = serializedObject.FindProperty("losesAggro");
        aggroLossTime = serializedObject.FindProperty("aggroLossTime");
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.PropertyField(startingHealth);
        EditorGUILayout.PropertyField(attackCycle);

        EditorGUILayout.PropertyField(useTargetingCircle);
        if (useTargetingCircle.boolValue) {
            EditorGUILayout.PropertyField(playerLayer);
            EditorGUILayout.PropertyField(targetingCircle);
        }

        EditorGUILayout.PropertyField(healthDropChance);
        if (healthDropChance.floatValue > 0.0f) {
            EditorGUILayout.PropertyField(healthDrop);
        }

        EditorGUILayout.PropertyField(hasAggroLoss);
        if (hasAggroLoss.boolValue) {
            EditorGUILayout.PropertyField(aggroLossTime);
        }

        serializedObject.ApplyModifiedProperties();
    }
}