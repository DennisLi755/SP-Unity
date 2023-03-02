using System;
using UnityEngine;

[Serializable]
public struct SkillInfo {
    public string Name;
    public string SkillDescription;
    public string SkillHint;
    public int ManaCost;
    public float Cooldown;
}

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Collections", fileName = "New Skill Collection")]
public class Skills : ScriptableObject {
    public SkillInfo[] skills;
}