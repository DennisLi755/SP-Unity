using UnityEditor;
using UnityEngine;

public class BossTrigger : Trigger {
    [SerializeField]
    private Vector3 BossSpawnPosition;
    [SerializeField]
    GameObject boss;

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(BossSpawnPosition + transform.position, new Vector3(1, 1));
    }

    protected override void OnTrigger() {
        boss.SetActive(true);
        base.OnTrigger();
    }
}