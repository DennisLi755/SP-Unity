using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BossTrigger : MonoBehaviour {
    protected BoxCollider2D trigger;
    [SerializeField]
    protected LayerMask playerLayer;
    [SerializeField]
    private Vector3 BossSpawnPosition;
    [SerializeField]
    GameObject boss;
    [SerializeField]
    GameObject wall;

#if UNITY_EDITOR
    /// <summary>
    /// Draws debug info to the screen
    /// </summary>
    private void OnDrawGizmos() {
        if (!EditorApplication.isPlaying && trigger == null) {
            trigger = GetComponent<BoxCollider2D>();
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(trigger.bounds.center, trigger.bounds.size);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(BossSpawnPosition + transform.position, new Vector3(1,1));
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    protected void Start() {
        trigger = GetComponent<BoxCollider2D>();
    }
    /// <summary>
    /// Detects whether or not the player interacts with the object
    /// </summary>
    protected virtual void Update() {
        RaycastHit2D hit = Physics2D.BoxCast(trigger.bounds.center, trigger.bounds.size, 0, Vector2.zero, 0, playerLayer);
        if (hit) {

        }
    }

    private void SpawnBoss() {
        gameObject.SetActive(false);
        wall.SetActive(true);
        Instantiate(boss, BossSpawnPosition + transform.position, Quaternion.identity);
    }
}