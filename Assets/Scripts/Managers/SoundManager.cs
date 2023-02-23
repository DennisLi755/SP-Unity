using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public struct SoundEffect {
    public string name;
    public AudioClip soundEffect;
}

public enum SoundSource {
    player,
    enemy,
    music,
    UI,
    environment
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance {
        get {
            if (instance == null) {
                instance = new SoundManager();
            }
            return instance;
        }
    }
    [SerializeField]
    private List<AudioSource> playerSources;
    [SerializeField]
    private AudioSource enemySource;
    [SerializeField]
    private AudioSource[] musicSource;
    [SerializeField]
    private AudioSource UISource;
    [SerializeField]
    private AudioSource environmentSource;

    [Range(0, 1)]
    [SerializeField]
    private float musicVolume;
    [Range(0, 1)]
    [SerializeField]
    private float soundEffectVolume;

    [SerializeField]
    SoundEffect[] soundEffectsArray;
    Dictionary<string, AudioClip> soundEffects;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //load volumes from player preferences

        soundEffects = new Dictionary<string, AudioClip>();
        foreach (SoundEffect se in soundEffectsArray) {
            soundEffects.Add(se.name, se.soundEffect);
        }
    }

    public AudioSource FindSource(SoundSource source) {
        switch(source) {
            case SoundSource.player:
                foreach (AudioSource se in playerSources) {
                    if (!se.isPlaying) {
                        return se;
                    }
                }
                Debug.LogError($"No Available Player SoundSource");
                return null;
            default:
                return null;
        }
    }

    public void PlaySoundEffect(string effectName, SoundSource source) {
        AudioSource audioSource = FindSource(source);
        audioSource.clip = soundEffects[effectName];
        audioSource.Play();
    }
}
