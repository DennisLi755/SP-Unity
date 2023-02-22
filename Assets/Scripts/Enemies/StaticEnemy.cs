using System;
using System.Collections;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct TargetingCircle {
    public Vector2 center;
    public float radius;
}

public class StaticEnemy : MonoBehaviour, IDamageable {

    [SerializeField]
    private int maxHealth;
    private int currentHealth;
    private bool isDamageable = true;
    public bool IsDamageable {get => isDamageable; set => isDamageable = value; }
    [SerializeField]
    private GameObject healthDrop;
    [SerializeField]
    private float healthDropChance;

    [SerializeField]
    protected bool useTargetingCircle;
    [SerializeField]
    protected TargetingCircle targetingCircle;
    protected BoxCollider2D bodyCollider;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    protected UnityEvent[] attackCycle;
    protected bool canAttack = false;
    public bool CanAttack {get => canAttack; set => canAttack = value; }
    protected bool canContinueAttack = false;
    public bool CanContinueAttack {get => canContinueAttack; set => canContinueAttack = value; }
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
        if (useTargetingCircle) {
            Handles.color = new Color(0, 1, 0);
            Handles.DrawWireDisc(transform.position + new Vector3(targetingCircle.center.x, targetingCircle.center.y), Vector3.forward, targetingCircle.radius);
        }
    }
#endif

    protected void Start() {
        if (!useTargetingCircle && GetComponent<SpriteRenderer>().isVisible) {
            canAttack = true;
            canContinueAttack = true;
        }
        bodyCollider = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
    }

    protected void Update() {
        if (useTargetingCircle) {
            RaycastHit2D hit = Physics2D.CircleCast(targetingCircle.center + (Vector2)transform.position, targetingCircle.radius, Vector2.zero, 0.0f, playerLayer);
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
                SpawnPickup(healthDropChance);
            }
        }
    }

    public void SpawnPickup(float chance) {
        float randomNum = UnityEngine.Random.Range(0, 100);
        if (randomNum <= chance) {
            Instantiate(healthDrop, transform.position, Quaternion.identity);
        }
    }
}