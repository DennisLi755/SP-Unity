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
    cutscene,
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
    private List<AudioSource> playerSources = new List<AudioSource>();
    [SerializeField]
    private AudioSource enemySource;
    private List<AudioSource> musicSources = new List<AudioSource>();
    private List<AudioSource> cutsceneSources = new List<AudioSource>();
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
    private SoundEffect[] soundEffectsArray;
    private Dictionary<string, AudioClip> soundEffects;
    private int currentLayer;

    private bool devMute = false;

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

    private void Update() {
        if (Input.GetKeyDown(KeyCode.M)) {
            devMute = !devMute;
            if (devMute) {
                musicSources[currentLayer].volume = 0.0f;
            }
            else {
                musicSources[currentLayer].volume = musicVolume;
            }
        }
    }

    public AudioSource FindSource(SoundSource source) {
        switch(source) {
            case SoundSource.player:
                if (playerSources.Count == 0) {
                    MakeSource(playerSources);
                }
                for (int i = 0; i < playerSources.Count; i++) {
                    if (!playerSources[i].isPlaying) {
                        return playerSources[i];
                    } else if (i == playerSources.Count-1) {
                        MakeSource(playerSources);
                    }
                }
                Debug.LogError($"No Available Player SoundSource");
                return null;
            case SoundSource.cutscene:
                if (cutsceneSources.Count == 0) {
                    MakeSource(cutsceneSources);
                }
                for (int i = 0; i < cutsceneSources.Count; i++) {
                    if (!cutsceneSources[i].isPlaying) {
                        return cutsceneSources[i];
                    } else if (i == cutsceneSources.Count-1) {
                        MakeSource(cutsceneSources);
                    }
                }
                Debug.LogError($"No Available Cutscene SoundSource");
                return null;

            case SoundSource.UI:
                return UISource;

            default:
                Debug.LogError($"The case for SoundSource {source} has not been setup yet");
                return null;
        }
    }

    public void MakeSource(List<AudioSource> sources) {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = soundEffectVolume;
        sources.Add(audioSource);
    }

    public void PlaySoundEffect(string effectName, SoundSource source) {
        AudioSource audioSource = FindSource(source);
        audioSource.clip = soundEffects[effectName];
        audioSource.Play();
        StartCoroutine(WaitForSoundToEnd());

        IEnumerator WaitForSoundToEnd() {
            while (audioSource.isPlaying) {
                yield return null;
            }

            if (source == SoundSource.player) {
                Destroy(audioSource);
                playerSources.Remove(audioSource);
            }
        }
    }

    public void PlayerUISoundEffect(string effectName) {
        PlaySoundEffect(effectName, SoundSource.UI);
    }

    public void SetUpMusicLayers(string[] names) {
        currentLayer = 0;
        float volume = musicVolume;
        for (int i = 0; i < names.Length; i++) {
            musicSources.Add(gameObject.AddComponent<AudioSource>());
            musicSources[i].clip = soundEffects[names[i]];
            musicSources[i].volume = volume;
            musicSources[i].Play();
            volume = 0.0f;
        }
    }

    public void ChangeMusicLayer(float fadeTime) {
        StartCoroutine(FadeOut(currentLayer, fadeTime, false));
        StartCoroutine(FadeIn(currentLayer+1, fadeTime));
    }

    public void FadeOutCurrentLayer(float fadeTime) {
        StartCoroutine(FadeOut(currentLayer, fadeTime, true));

    }

    IEnumerator FadeOut(int layer, float fadeTime, bool resetLayers) {
        AudioSource audioSource = musicSources[currentLayer];
        float startVolume = audioSource.volume;

        while (audioSource.volume >= 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.volume = 0;
        if (resetLayers) {
            ResetMusicLayers();
        }
    }

    IEnumerator FadeIn(int layer, float fadeTime) {
        AudioSource audioSource = musicSources[currentLayer + 1];
        float endVolume = musicVolume;
        Debug.Log(musicVolume);
        while (audioSource.volume < endVolume) {
            audioSource.volume += endVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.volume = endVolume;
        currentLayer++;
    }

    public void ResetMusicLayers() {
        for (int i = 0; i < musicSources.Count;) {
            Destroy(musicSources[i]);
            musicSources.RemoveAt(i);
        }
    }
}
