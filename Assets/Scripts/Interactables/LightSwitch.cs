using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSwitch : TextboxInteraction {
    private new void Start() {
        base.Start();
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
                bool turnLightOn = !(SceneLights.Instance.GetLightIntensity("Global Lighting") == 1);
                SceneLights.Instance.SetLightIntensity("Global Lighting", turnLightOn? 1 : 0.5f);
                if (turnLightOn) {
                    SoundManager.Instance.PlaySoundEffect("LightSwitch1", SoundSource.environment);
                } else {
                    SoundManager.Instance.PlaySoundEffect("LightSwitch2", SoundSource.environment);
                }
            }
        }
    }
}