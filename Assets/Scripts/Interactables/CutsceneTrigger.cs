using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(BoxCollider2D))]
public class CutsceneTrigger : MonoBehaviour
{
    protected BoxCollider2D trigger;
    [SerializeField]
    protected LayerMask playerLayer;
    [SerializeField]
    private string cutsceneName;
    private bool triggered;

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
        Gizmos.DrawWireCube(transform.position, new Vector3(1,1));
    }
    #endif

    // Start is called before the first frame update
    protected void Start()
    {
        trigger = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    protected void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(trigger.bounds.center, trigger.bounds.size, 0, Vector2.zero, 0, playerLayer);
        if (hit && !triggered) {
            DialogueManager.Instance.StartDialogue(cutsceneName);
            triggered = true;
        } else if (!hit && triggered) {
            triggered = false;
        }
    }
}
