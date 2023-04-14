using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour {

    [SerializeField]
    private bool staticCamera = false;
    [SerializeField]
    private Vector3 cameraPosition;
    [SerializeField]
    public UnityEvent onEnter;
    [SerializeField]
    public UnityEvent onExit;

    void Start() {
        if (cameraPosition == Vector3.zero) {
            Debug.LogWarning($"{gameObject.name} has no camera position");
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
    }

    [ContextMenu("Move Player Here")]
    public void MovePlayerHere() {
        FindObjectOfType<PlayerInfo>().transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0.0f);
    }
    /// <summary>
    /// Plays a Sound effect through the cutscene sound source
    /// </summary>
    /// <param name="soundEffect"></param>
    public void PlaySoundEffect(string soundEffect) {
        SoundManager.Instance.PlaySoundEffect(soundEffect, SoundSource.cutscene);
    }
    /// <summary>
    /// Stops a sound
    /// </summary>
    public void StopSound() {
        SoundManager.Instance.StopSoundSource(SoundSource.cutscene);
    }
}