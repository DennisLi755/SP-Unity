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
    //this is used to make sure the player doesn't active the transition multiple times
    private bool transitioning = false;

    public Vector3 TransitionToLocation { get => transitionToLocation + transform.position; }

    private void OnDrawGizmos() {
        //draws the location the player will be after being transitioned to this transition
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + transitionToLocation, new Vector3(0.376792f * 2, 0.2201519f * 2, 0));

        //draws the area the player needs to enter to active that transition
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transitionZone.center, transitionZone.size);
    }

    void Start() { }

    
    void Update() {
        RaycastHit2D cast = Physics2D.BoxCast((transitionZone.center + transform.position), transitionZone.size, 0.0f, Vector2.zero, 0.0f, playerLayer);
        if (cast && !transitioning) {
            Transition();
            transitioning = true;
        }
    }

    /// <summary>
    /// Transitions the player to the transition partner transition
    /// </summary>
    public void Transition() {
        //stop the player from moving and fade to blade
        PlayerInfo.Instance.PlayerControl.Freeze();
        UIManager.Instance.FadeToBlack();
        StartCoroutine(WaitForFade());

        IEnumerator WaitForFade() {
            yield return new WaitForSecondsRealtime(1.0f);
            PlayerInfo.Instance.transform.position = transitionPartner.TransitionToLocation;
            transitionPartner.GetComponentInParent<Room>().MoveTo();

            yield return new WaitForSeconds(0.5f);
            transitioning = false;
        }
    }

    /// <summary>
    /// Moves the player to the location they will be after transitioning to this transition
    /// </summary>
    [ContextMenu("Move Player to Transition")]
    public void MovePlayerToTransitionLocation() {
        GameObject player = FindObjectOfType<PlayerInfo>().gameObject;
        Vector3 offset = player.GetComponent<BoxCollider2D>().offset;
        player.transform.position = transitionToLocation - offset;
    }
}