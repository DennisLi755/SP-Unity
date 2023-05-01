using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : InteractableObject {
    [SerializeField]
    private Vector3 playerPosition;
    public Vector3 PlayerPosition => playerPosition + transform.position;

#if UNITY_EDITOR
    new void OnDrawGizmos() {
        base.OnDrawGizmos();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(playerPosition + transform.position, new Vector3(0.491736591f, 0.384848833f, 0.0f));
    }
#endif

    public override void OnInteract() {
        GameManager.Instance.SavePlayerData(gameObject.name);
        PlayerInfo.Instance.EnableTutorialText("Saved!");
        PlayerInfo.Instance.Heal(99);
        StartCoroutine(TurnOffSaveText());
    }

    IEnumerator TurnOffSaveText() {
        yield return new WaitForSeconds(1.5f);
        PlayerInfo.Instance.DisableTopTutorialText();
    }
}