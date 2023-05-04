using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct FlagEvents {
    public string flag;
    public UnityEvent[] ev;
    }

public class Room : MonoBehaviour {

    [SerializeField]
    private bool staticCamera = false;
    [SerializeField]
    private Vector3 cameraPosition;
    public UnityEvent onEnter;
    public UnityEvent onExit;

    [SerializeField]
    private FlagEvents[] flagsArray;
    private Dictionary<string, UnityEvent[]> flagsDictionary;

    void Start() {
        flagsDictionary = new Dictionary<string, UnityEvent[]>();
        foreach (FlagEvents flagEvents in flagsArray) {
            flagsDictionary.Add(flagEvents.flag, flagEvents.ev);
        }

        if (cameraPosition == Vector3.zero) {
            Debug.LogWarning($"{gameObject.name} has no camera position");
        }
    }

    public void UpdateEvents(string lastProgressionFlag)
    {
        if (flagsDictionary.ContainsKey(lastProgressionFlag)) {
            onEnter = flagsDictionary[lastProgressionFlag][0];
            onExit = flagsDictionary[lastProgressionFlag][1];
        }
    }
    /// <summary>
    /// Called when the player is moved to the room; tells the camera whether it should follow the character and pre-positions the camera
    /// </summary>
    public void UpdateCameraFollow() {
        CameraManager.Instance.FollowTarget(!staticCamera);
        if (staticCamera) {
            CameraManager.Instance.MoveTo(cameraPosition);
        }
        else {
            CameraManager.Instance.MoveTo(PlayerInfo.Instance.transform.position);
        }
    }

    /// <summary>
    /// Moves the camera to this room
    /// </summary>
    [ContextMenu("Move Camera to This Room")]
    public void MoveCameraHere() {
        Camera.main.transform.position = cameraPosition;

        Camera.main.GetComponent<CameraManager>().FollowTarget(!staticCamera);
        GameManager.MarkSceneDirty();
    }

    [ContextMenu("Move Player Here")]
    public void MovePlayerHere() {
        FindObjectOfType<PlayerInfo>().transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0.0f);
        GameManager.MarkSceneDirty();
    }
    /// <summary>
    /// Plays a Sound effect through the environment sound source
    /// </summary>
    /// <param name="soundEffect"></param>
    public void PlaySoundEffect(string soundEffect) {
        SoundManager.Instance.PlaySoundEffect(soundEffect, SoundSource.environment);
    }
    /// <summary>
    /// Stops a sound
    /// </summary>
    public void StopSound() {
        SoundManager.Instance.StopSoundSource(SoundSource.environment);
    }
}