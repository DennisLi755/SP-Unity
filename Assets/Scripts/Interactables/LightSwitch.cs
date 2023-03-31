using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSwitch : TextboxInteraction {

    private bool lightOn;
    [SerializeField]
    private GameObject lightObject;

    private new void Start() {
        base.Start();
        lightOn = lightObject.activeSelf;
    }
    /// <summary>
    /// OnInteract() for light switch
    /// Change the state of the lights
    /// </summary>
    public override void OnInteract() {
        if (PlayerInfo.Instance.PlayerControl.FacingDirection == FacingDirection.Up) {
            if (GameManager.Instance.GetProgressionFlag("Second Awake")) {
                base.OnInteract();
            } else {
                lightOn = !lightOn;
                lightObject.SetActive(lightOn);
                if (lightOn) {
                    SoundManager.Instance.PlaySoundEffect("LightSwitch1", SoundSource.environment);
                } else {
                    SoundManager.Instance.PlaySoundEffect("LightSwitch2", SoundSource.environment);
                }
            }
        }
    }
}