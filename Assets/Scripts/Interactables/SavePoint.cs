using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : InteractableObject {
    [SerializeField]
    private Vector3 playerPosition;
    public Vector3 PlayerPosition;

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(PlayerPosition + transform.position, new Vector3(0.491736591f, 0.384848833f, 0.0f));
    }

    public override void OnInteract() {
        GameManager.Instance.SavePlayerData(gameObject.name);
    }
}