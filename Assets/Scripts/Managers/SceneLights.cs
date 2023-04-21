using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SceneLights : MonoBehaviour {
    private static SceneLights instance;
    public static SceneLights Instance => instance;

    private Dictionary<string, Light2D> lights = new Dictionary<string, Light2D>();

    private void Awake() {
        //asume that if a new instance is created, the old one is from an old scene that was not properly destroyed
        if (instance != null) {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    private void Start() {
        foreach (Light2D l in transform.GetComponentsInChildren<Light2D>()) {
            lights.Add(l.gameObject.name, l);
        }
    }

    public void SetLightIntensity(string lightName, float intensity) {
        try {
            lights[lightName].intensity = intensity;
        } catch (KeyNotFoundException) {
            Debug.LogError("Light " + lightName + " not found");
        }
    }

    public float GetLightIntensity(string lightName) {
        try {
            return lights[lightName].intensity;
        } catch (KeyNotFoundException) {
            Debug.LogError("Light " + lightName + " not found");
            return -1;
        }
    }
}