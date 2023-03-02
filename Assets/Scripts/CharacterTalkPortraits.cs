using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TalkPortrait {
    public string name;
    public Sprite expression;
}

[CreateAssetMenu(menuName = "Scriptable Objects/Character Talk Portrait Collection", fileName ="New Talk Portaits")]
public class CharacterTalkPortraits : ScriptableObject {
    public string[] aliases;
    public TalkPortrait[] portraits;
}