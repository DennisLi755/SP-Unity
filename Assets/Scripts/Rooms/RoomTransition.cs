using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoomTransition : MonoBehaviour {

    [SerializeField]
    private RoomTransition transitionPartner;
    [SerializeField]
    private Vector3 transitionToLocation;
    [SerializeField]
    private Bounds transitionZone;
    [SerializeField]
    private LayerMask playerLayer;
    private bool transitioning = false;

    public Vector3 TransitionToLocation { get => transitionToLocation + transform.position; }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + transitionToLocation, new Vector3(0.376792f * 2, 0.2201519f * 2, 0));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transitionZone.center, transitionZone.size);
    }

    void Start() {

    }

    
    void Update() {
        RaycastHit2D cast = Physics2D.BoxCast((transitionZone.center + transform.position), transitionZone.size, 0.0f, Vector2.zero, 0.0f, playerLayer);
        if (cast && !transitioning) {
            Transition();
            transitioning = true;
        }
    }

    public void Transition() {
        PlayerInfo.Instance.PlayerControl.Freeze();
        UIManager.Instance.FadeToBlack();
        StartCoroutine(WaitForFade());

        IEnumerator WaitForFade() {
            yield return new WaitForSecondsRealtime(1.0f);
            PlayerInfo.Instance.transform.position = transitionPartner.TransitionToLocation;
            transitionPartner.GetComponentInParent<Room>().MoveTo();
            transitioning = false;
        }
    }

    [ContextMenu("Move Player to Transition")]
    public void MovePlayerToTransitionLocation() {
        GameObject player = FindObjectOfType<PlayerInfo>().gameObject;
        Vector3 offset = player.GetComponent<BoxCollider2D>().offset;
        player.transform.position = transitionToLocation - offset;
    }
}