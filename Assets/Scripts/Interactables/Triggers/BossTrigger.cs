using UnityEngine;

public class BossTrigger : Trigger {
    [SerializeField]
    private Vector3 BossSpawnPosition;
    [SerializeField]
    GameObject boss;
    #if UNITY_EDITOR
    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(BossSpawnPosition + transform.position, new Vector3(1, 1));
    }
    #endif

    protected override void OnTrigger() {
        boss.SetActive(true);
        base.OnTrigger();
    }
}