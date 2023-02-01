using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Room : MonoBehaviour {

    [SerializeField]
    private bool staticCamera = false;
    [SerializeField]
    [DraggablePoint]
    private Vector3 cameraPosition;

    void Start() {

    }

    /// <summary>
    /// Called when the player is moved to the room; tells the camera whether it should follow the character and pre-positions the camera
    /// </summary>
    public void MoveTo() {
        CameraManager.Instance.FollowPlayer(!staticCamera);
        if (staticCamera) {
            CameraManager.Instance.MoveTo(cameraPosition);
        }
        else {
            CameraManager.Instance.MoveTo(PlayerInfo.Instance.transform.position);
        }
        UIManager.Instance.FadeFromBlack();
        PlayerInfo.Instance.PlayerControl.CanMove = true;
    }

    /// <summary>
    /// Moves the camera to this room
    /// </summary>
    [ContextMenu("Move Camera to This Room")]
    public void MoveCameraHere() {
        Camera.main.transform.position = cameraPosition;

        Camera.main.GetComponent<CameraManager>().FollowPlayer(!staticCamera);
    }
}