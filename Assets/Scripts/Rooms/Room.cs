using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Room : MonoBehaviour {

    [SerializeField]
    private bool staticCamera = false;
    [SerializeField]
    private Vector3 cameraPosition;

    void Start() {

    }

    public void MoveTo() {
        CameraManager.Instance.FollowPlayer(!staticCamera);
        if (staticCamera) {
            CameraManager.Instance.MoveTo(cameraPosition);
        }
        UIManager.Instance.FadeFromBlack();
        PlayerInfo.Instance.PlayerControl.CanMove = true;
    }
}