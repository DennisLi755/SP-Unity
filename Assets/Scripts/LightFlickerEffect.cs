using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class LightFlickerEffect : MonoBehaviour {
    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    [SerializeField]
    private new Light2D light;
    [SerializeField]
    private float intensityRange;
    [SerializeField]
    private float timeMin;
    [SerializeField]
    private float timeMax;
    [SerializeField]
    public bool flickerIntensity;
    private float baseIntensity;

    public void Start() {
        if (light == null) {
            light = GetComponent<Light2D>();
        }
        baseIntensity = light.intensity;
        StartCoroutine(FlickIntensity());
    }

    private IEnumerator FlickIntensity() {
        float t0 = Time.time;
        float t = t0;
        WaitUntil wait = new WaitUntil(() => Time.time > t0 + t);
        yield return new WaitForSeconds(Random.Range(0.01f, 0.5f));

        while (true) {
            if (flickerIntensity) {
                t0 = Time.time;
                float r = Random.Range(baseIntensity - intensityRange, baseIntensity + intensityRange);
                light.intensity = r;
                t = Random.Range(timeMin, timeMax);
                yield return wait;
            }
            else yield return null;
        }
    }
}