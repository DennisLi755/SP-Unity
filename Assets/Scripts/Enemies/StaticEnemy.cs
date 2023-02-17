using System;
using System.Collections;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class StaticEnemy : MonoBehaviour, IDamageable {

    [SerializeField]
    private int maxHealth;
    private int currentHealth;
    private bool isDamageable = true;
    public bool IsDamageable {get => isDamageable; set => isDamageable = value; }

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

#if UNITY_EDITOR
    protected void OnDrawGizmos() {
        GUIContent content = new GUIContent($"Health: {currentHealth}");
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        Handles.Label(new Vector3(transform.position.x - 0.75f, transform.position.y + 1.0f, transform.position.z), content, style);
    }
#endif

    protected void Start() {
        if (useTargetingCircle) {
            targetingCircle = GetComponent<CircleCollider2D>();
        }
        else {
            if (GetComponent<SpriteRenderer>().isVisible) {
                canAttack = true;
                canContinueAttack = true;
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
        if (isDamageable) {
            currentHealth -= amount;
            if (currentHealth <= 0) {
                gameObject.SetActive(false);
            }
        }
    }
}