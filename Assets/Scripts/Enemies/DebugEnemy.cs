using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Events;

public class DebugEnemy : MonoBehaviour {

    [SerializeField]
    private UnityEvent[] attackCycle;
    private bool canContinueAttack = false;
    private int attackCycleIndex = 0;

    void Start() {

    }

    void Update() {
        if (canContinueAttack) {
            attackCycle[attackCycleIndex]?.Invoke();
            attackCycleIndex++;
            if (attackCycleIndex >= attackCycle.Length) {
                attackCycleIndex = 0;
            }
        }
    }

    [ContextMenu("Start Attacking")]
    private void StartAttackCycle() {
        canContinueAttack = true;
    }

    public void ShootPatternBullet(GameObject pattern) {
        Instantiate(pattern, transform);
    }

    public void Wait(float time) {
        if (time == 0.0f) {
            return;
        }
        canContinueAttack = false;
        StartCoroutine(WaitForCycle(time));

        IEnumerator WaitForCycle(float time) {
            yield return new WaitForSeconds(time);
            canContinueAttack = true;
        }
    }
}