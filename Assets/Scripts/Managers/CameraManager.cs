using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    private CameraManager instance;
    public CameraManager Instance {
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
    private bool shouldFollowPlayer = true;

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

    public void MoveTo(Vector3 position) {
        transform.position = position;
    }

    public void FollowPlayer(bool follow) {
        shouldFollowPlayer = follow;
    }
}