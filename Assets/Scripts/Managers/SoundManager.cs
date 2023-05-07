using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class SoundManager : MonoBehaviour {
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
    [SerializeField]
    private List<AudioSource> musicSources = new List<AudioSource>();
    [SerializeField]
    private List<AudioSource> cutsceneSources = new List<AudioSource>();
    [SerializeField]
    private List<AudioSource> UISources = new List<AudioSource>();
    [SerializeField]
    private List<AudioSource> environmentSources = new List<AudioSource>();

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
    void Start() {
        //load volumes from player preferences

        soundEffects = new Dictionary<string, AudioClip>();
        foreach (SoundEffect se in soundEffectsArray) {
            soundEffects.Add(se.name, se.soundEffect);
        }
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = soundEffectVolume;
        UISources.Add(audioSource);
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

    public bool FindPlayingSource(SoundSource source, out AudioSource audioSource) {
        audioSource = null;
        switch (source) {
            case SoundSource.player:
                for (int i = 0; i < playerSources.Count; i++) {
                    if (playerSources[i].isPlaying) {
                        audioSource = playerSources[i];
                        return true;
                    }
                }
                Debug.LogError($"No Playing Player SoundSource");
                return false;
            case SoundSource.cutscene:
                for (int i = 0; i < cutsceneSources.Count; i++) {
                    if (cutsceneSources[i].isPlaying) {
                        audioSource = cutsceneSources[i];
                        return true;
                    }
                }
                Debug.LogError($"No Playing Cutscene SoundSource");
                return false;

            case SoundSource.UI:
                for (int i = 0; i < UISources.Count; i++) {
                    if (UISources[i].isPlaying) {
                        audioSource = UISources[i];
                        return true;
                    }
                }
                Debug.LogError($"No Playing UI SoundSource");
                return false;
            case SoundSource.environment:
                for (int i = 0; i < environmentSources.Count; i++) {
                    if (environmentSources[i].isPlaying) {
                        audioSource = environmentSources[i];
                        return true;
                    }
                }
                Debug.LogError($"No Playing Environment SoundSource");
                return false;

            default:
                Debug.LogError($"The case for SoundSource {source} has not been setup yet");
                return false;
        }
    }

    /// <summary>
    /// Attaches a new audio source to the GameObject
    /// </summary>
    /// <param name="sources"></param>
    public AudioSource MakeSource(List<AudioSource> sources) {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = soundEffectVolume;
        sources.Add(audioSource);
        return audioSource;
    }

    public void PlayTextSound(string sound) {
        UISources[0].clip = soundEffects[sound];
        UISources[0].Play();
    }

    public void PlayUISound(string sound) {
        PlaySoundEffect(sound, SoundSource.UI);
    }

    /// <summary>
    /// Plays the given sound effect through an audio source coresponding to the given sound source
    /// </summary>
    /// <param name="effectName"></param>
    /// <param name="source"></param>
    public void PlaySoundEffect(string effectName, SoundSource source) {
        List<AudioSource> currentSource = new List<AudioSource>();
        switch (source) {
            case SoundSource.player: currentSource = playerSources; break;
            case SoundSource.environment: currentSource = environmentSources; break;
            case SoundSource.cutscene: currentSource = cutsceneSources; break;
            case SoundSource.UI: currentSource = UISources; break;
        }

        AudioSource audioSource = MakeSource(currentSource);
        audioSource.clip = soundEffects[effectName];
        audioSource.Play();
        StartCoroutine(WaitForSoundToEnd());

        IEnumerator WaitForSoundToEnd() {
            while (audioSource.isPlaying) {
                yield return null;
            }

            switch (source) {
                case SoundSource.player: playerSources.Remove(audioSource); break;
                case SoundSource.environment: environmentSources.Remove(audioSource); break;
                case SoundSource.UI: UISources.Remove(audioSource); break;
            }
            Destroy(audioSource);
        }
    }

    public void StopSoundSource(SoundSource source) {
        AudioSource audioSource;
        if (!FindPlayingSource(source, out audioSource)) {
            return;
        }
        audioSource.Stop();
        switch (source) {
            case SoundSource.player:
                playerSources.Remove(audioSource);
                break;
            case SoundSource.cutscene:
                cutsceneSources.Remove(audioSource);
                break;
            case SoundSource.environment:
                environmentSources.Remove(audioSource);
                break;
            case SoundSource.music:
                musicSources.Remove(audioSource);
                break;
        }
        Destroy(audioSource);
    }

    /// <summary>
    /// Sets up a music audio source for each item in the array to use for layer blending
    /// </summary>
    /// <param name="names"></param>
    public void SetUpMusicLayers(string[] names, float volScalar = 1) {
        currentLayer = 0;
        float volume = musicVolume * volScalar;
        for (int i = 0; i < names.Length; i++) {
            musicSources.Add(gameObject.AddComponent<AudioSource>());
            musicSources[i].clip = soundEffects[names[i]];
            musicSources[i].volume = volume;
            musicSources[i].loop = true;
            musicSources[i].Play();
            volume = 0.0f;
        }
    }

    /// <summary>
    /// Fades to the next music layer in the list
    /// </summary>
    /// <param name="fadeTime"></param>
    public void ChangeMusicLayer(float fadeTime) {
        StartCoroutine(FadeOut(currentLayer, fadeTime, false));
        StartCoroutine(FadeIn(currentLayer + 1, fadeTime));
    }

    /// <summary>
    /// Fades out the current music layer without fading in a new one
    /// </summary>
    /// <param name="fadeTime"></param>
    public void FadeOutCurrentLayer(float fadeTime) {
        StartCoroutine(FadeOut(currentLayer, fadeTime, true));
    }

    public void FadeOutSource(float fadeTime, SoundSource source) {
        StartCoroutine(FadeSource(fadeTime, source));
    }

    IEnumerator FadeSource(float fadeTime, SoundSource source) {
        AudioSource audioSource;
        if (!FindPlayingSource(source, out audioSource)) {
            yield break;
        }
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.volume = 0;
    }

    /// <summary>
    /// Fades out the given music layer over the given time
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="fadeTime"></param>
    /// <param name="resetLayers"></param>
    /// <returns></returns>
    IEnumerator FadeOut(int layer, float fadeTime, bool resetLayers) {
        AudioSource audioSource = musicSources[layer];
        if (audioSource == null) {
            yield break;
        }
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.volume = 0;
        if (resetLayers) {
            ResetMusicLayers();
        }
    }

    /// <summary>
    /// Fades the given music layer in over the given time
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="fadeTime"></param>
    /// <returns></returns>
    IEnumerator FadeIn(int layer, float fadeTime) {
        AudioSource audioSource = musicSources[layer];
        float endVolume = musicVolume;
        while (audioSource.volume < endVolume) {
            audioSource.volume += endVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.volume = endVolume;
        currentLayer++;
    }

    /// <summary>
    /// Resets all music layers
    /// </summary>
    public void ResetMusicLayers() {
        for (int i = 0; i < musicSources.Count;) {
            Destroy(musicSources[i]);
            musicSources.RemoveAt(i);
        }
    }

    public void ChangeMusicVolScalar(float fadeTime, float scalar) {
        StartCoroutine(ChangeVolume(fadeTime, scalar));
    }

    IEnumerator ChangeVolume(float fadeTime, float volume) {
        AudioSource audioSource = musicSources[currentLayer];
        if (audioSource == null) {
            yield break;
        }
        float endVolume = musicVolume * volume;
        Debug.Log("End Volume: " + endVolume);

        bool exit;
        if (audioSource.volume < endVolume) {
            while (audioSource.volume < endVolume) {
                audioSource.volume += endVolume * Time.deltaTime / fadeTime;
                Debug.Log(audioSource.volume);
                yield return null;
            }
        } else {
            while (audioSource.volume > endVolume) {
                audioSource.volume -= endVolume * Time.deltaTime / fadeTime;
                Debug.Log(audioSource.volume);
                yield return null;
            }
        }

        audioSource.volume = endVolume;
    }
}
