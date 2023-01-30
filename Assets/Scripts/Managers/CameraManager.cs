using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    private static CameraManager instance;
    public static CameraManager Instance {
        get {
            if (instance == null) {
                instance = new CameraManager();
            }
            return instance;
        }
    }

    private Transform target;
    private Vector3 offset = new Vector3(0, 0, -10);
    private const float damping = 0.1f;
    private bool shouldFollowPlayer = false;

    private Vector3 velocity = Vector3.zero;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        target = PlayerInfo.Instance.transform;   
    }

    void FixedUpdate() {
        if (shouldFollowPlayer) {
            Vector3 movePosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);
        }
    }

    /// <summary>
    /// Moves the camera to the specified position in the world
    /// </summary>
    /// <param name="position"></param>
    public void MoveTo(Vector3 position) {
        transform.position = position;
    }

    /// <summary>
    /// Enables player following behavior
    /// </summary>
    /// <param name="follow"></param>
    public void FollowPlayer(bool follow) {
        shouldFollowPlayer = follow;
    }
}