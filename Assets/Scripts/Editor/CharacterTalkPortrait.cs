using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TalkPortrait {
    public string name;
    public Sprite expression;
}

[CreateAssetMenu(fileName ="Character Talk Portrait Collection")]
public class CharacterTalkPortraits : ScriptableObject {
    public string[] aliases;
    public TalkPortrait[] portraits;
}