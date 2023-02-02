using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class StaticEnemy : MonoBehaviour {

    [SerializeField]
    protected bool useTargetingCircle;
    protected CircleCollider2D targetingCircle;
    [SerializeField]
    LayerMask playerLayer;
    [SerializeField]
    protected UnityEvent[] attackCycle;
    protected bool canAttack = false;
    protected bool canContinueAttack = false;
    protected int attackCycleIndex = 0;

    protected void Start() {
        if (useTargetingCircle) {
            targetingCircle = GetComponent<CircleCollider2D>();
        }
        else {
            if (GetComponent<SpriteRenderer>().isVisible) {
                canAttack = true;
            }
        }
    }

    protected void Update() {
        if (useTargetingCircle) {
            RaycastHit2D hit = Physics2D.CircleCast(targetingCircle.bounds.center, targetingCircle.radius, Vector2.zero, 0.0f, playerLayer);
            if (hit && !canAttack) {
                canAttack = true;
                canContinueAttack = true;
            }
            if (!hit && canAttack) {
                canAttack = false;
            }
        }
        else {
            if (!canAttack && GetComponent<SpriteRenderer>().isVisible) {
                canAttack = true;
                canContinueAttack = true;
            }
            else if (canAttack && !GetComponent<SpriteRenderer>().isVisible) {
                canAttack = false;
            }
        }

        if (canContinueAttack && attackCycle.Length > 0) {
            attackCycle[attackCycleIndex]?.Invoke();
            attackCycleIndex++;
            if (attackCycleIndex >= attackCycle.Length) {
                attackCycleIndex = 0;
            }
        }
    }

    [ContextMenu("Start Attacking")]
    protected void StartAttackCycle() {
        canContinueAttack = true;
    }

    public void ShootPatternBullet(GameObject pattern) {
        Instantiate(pattern, transform.position, Quaternion.identity, BulletHolder.Instance.transform);
    }

    public void Wait(float time) {
        if (time == 0.0f) {
            return;
        }
        canContinueAttack = false;
        StartCoroutine(WaitForCycle(time));

        IEnumerator WaitForCycle(float time) {
            yield return new WaitForSeconds(time);
            if (canAttack) {
                canContinueAttack = true;
            }
        }
    }
}