using System;
using System.Collections;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class StaticEnemy : MonoBehaviour {

    [SerializeField]
    private int maxHealth;
    private int currentHealth;

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
    protected Coroutine attackCycleRoutine;
    protected bool overridePatternSpeed = false;
    protected float newPatternSpeed;

    protected void OnDrawGizmos() {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        Handles.Label(new Vector3(transform.position.x - 0.5f, transform.position.y + 1.0f, transform.position.z), $"Health: {currentHealth}", style);
    }

    protected void Start() {
        if (useTargetingCircle) {
            targetingCircle = GetComponent<CircleCollider2D>();
        }
        else {
            if (GetComponent<SpriteRenderer>().isVisible) {
                canAttack = true;
            }
        }
        currentHealth = maxHealth;
    }

    protected void Update() {
        if (useTargetingCircle) {
            RaycastHit2D hit = Physics2D.CircleCast(targetingCircle.bounds.center, targetingCircle.radius, Vector2.zero, 0.0f, playerLayer);
            if (hit && !canAttack) {
                canAttack = true;
                if (attackCycleRoutine == null) {
                    canContinueAttack = true;
                }
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
            overridePatternSpeed = false;
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

    public virtual void ShootPatternBullet(GameObject pattern) {
        GameObject pat = Instantiate(pattern, transform.position, Quaternion.identity, BulletHolder.Instance.transform);
        BulletPattern bulPat;
        if (pat.TryGetComponent<BulletPattern>(out bulPat) && overridePatternSpeed) {
            bulPat.SetSpeedOverride(newPatternSpeed);
        }
    }

    public void Wait(float time) {
        if (time == 0.0f) {
            return;
        }
        canContinueAttack = false;
        attackCycleRoutine = StartCoroutine(WaitForCycle(time));

        IEnumerator WaitForCycle(float time) {
            yield return new WaitForSeconds(time);
            if (canAttack) {
                canContinueAttack = true;
            }
            attackCycleRoutine = null;
        }
    }

    public void SetPatternSpeed(float speed) {
        overridePatternSpeed = true;
        newPatternSpeed = speed;
    }

    public void Hurt(int amount) {
        currentHealth -= amount;
        if (currentHealth <= 0) {
            gameObject.SetActive(false);
        }
    }
}