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
    public Transform Target {get => target; set => target = value;}
    private Vector3 offset = new Vector3(0, 0, -10);
    private const float damping = 0.1f;
    [SerializeField]
    private bool shouldFollowTarget = true;

    private Vector3 velocity = Vector3.zero;

    private void Awake() {
        if (instance == null) {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        target = PlayerInfo.Instance.transform;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            //for testing stuff
        }
    }

    void FixedUpdate() {
        
        if (shouldFollowTarget) {
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

    public void PanTo(Vector3 position, float time) {
        FollowTarget(false);
        float speed = Vector3.Distance(transform.position, position) / time;
        StartCoroutine(Pan());

        IEnumerator Pan() {
            while (transform.position != position) {
                transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void SetNewTarget(Transform transform) {
        FollowTarget(true);
        target = transform;
    }

    /// <summary>
    /// Enables player following behavior
    /// </summary>
    /// <param name="follow"></param>
    public void FollowTarget(bool follow) {
        shouldFollowTarget = follow;
    }

    /// <summary>
    /// Shakes the camera in place for the given amount of time; regular intensity is 1
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="time"></param>
    public void ScreenShake(float intensity, float time) {
        intensity /= 10;
        StartCoroutine(Shake());

        IEnumerator Shake() {
            Vector3 initialPosition = transform.position;
            float t = 0.0f;
            while (t < time) {
                Vector3 newPosition = initialPosition + new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), -10);
                transform.position = newPosition;

                t += Time.deltaTime;
                yield return null;
            }

            transform.position = initialPosition;
        }
    }
}