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

    #region Health & Damage
    [SerializeField]
    private int startingHealth;
    private int currentHealth;
    private bool isDamageable = true;

    [Range(0,100)]
    [SerializeField]
    private float healthDropChance;
    [SerializeField]
    private GameObject healthDrop;
    #endregion

    #region Attacking
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
    protected bool canContinueAttack = false;
    protected int attackCycleIndex = 0;
    protected Coroutine attackCycleRoutine;

    [SerializeField]
    protected bool losesAggro = true;
    [SerializeField]
    protected float aggroLossTime = 5.0f;
    protected Coroutine aggroLossRoutine;

    protected bool overridePatternSpeed = false;
    protected float newPatternSpeed;
    #endregion

#if UNITY_EDITOR
    protected void OnDrawGizmos() {
        //display health
        GUIContent content = new GUIContent($"Health: {currentHealth}");
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        Handles.Label(new Vector3(transform.position.x - 0.75f, transform.position.y + 1.0f, transform.position.z), content, style);
        //if the enemy uses a targetting circle, draw it
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
        currentHealth = startingHealth;
    }

    protected void Update() {
        if (useTargetingCircle) {
            //enemies that use targeting circles needs to check for the player within their targeting range
            RaycastHit2D hit = Physics2D.CircleCast(targetingCircle.center + (Vector2)transform.position, targetingCircle.radius, Vector2.zero, 0.0f, playerLayer);
            if (hit && !canAttack) {
                canAttack = true;
                //if the enemy's attack cycle is currently interrupted (such as a wait) do not immediately let it start attacking
                if (attackCycleRoutine == null) {
                    canContinueAttack = true;
                }
                //disable the enemy losing aggro
                if (aggroLossRoutine != null) {
                    StopCoroutine(aggroLossRoutine);
                    aggroLossRoutine = null;
                }
            }
            //player is no longer in the targeting range
            if (!hit && canAttack) {
                canAttack = false;
                canContinueAttack = false;
                if (losesAggro) {
                    aggroLossRoutine = StartCoroutine(ResetAttackCycle());
                }
            }
        }
        //enemies that shoot whenever on the screen
        else {
            //enemy is now on screen and can start attacking
            if (!canAttack && GetComponent<SpriteRenderer>().isVisible) {
                canAttack = true;
                if (attackCycleRoutine == null) {
                canContinueAttack = true;
                }
                //disable the enemy losing aggro
                if (aggroLossRoutine != null) {
                    StopCoroutine(aggroLossRoutine);
                    aggroLossRoutine = null;
                }
            }
            //enemy is no longer visible and should stop shooting
            else if (canAttack && !GetComponent<SpriteRenderer>().isVisible) {
                canAttack = false;
                canContinueAttack = false;
                if (losesAggro) {
                    aggroLossRoutine = StartCoroutine(ResetAttackCycle());
                }
            }
        }

        //if the enemy's attack cycle actually has data and they can continue attacking, then attack
        if (canContinueAttack && attackCycle.Length > 0) {
            //default assume the enemy uses the built in speed of the pattern
            overridePatternSpeed = false;
            //activate all of the actions associated with the current attack cycle index then increase the index for the next call
            attackCycle[attackCycleIndex]?.Invoke();
            attackCycleIndex++;
            //loop back to the start
            if (attackCycleIndex >= attackCycle.Length) {
                attackCycleIndex = 0;
            }
        }
    }

    /// <summary>
    /// Resets the enemy's attack information after the set amount of time has passed
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetAttackCycle() {
        yield return new WaitForSeconds(aggroLossTime);
        attackCycleIndex = 0;
        attackCycleRoutine = null;
        aggroLossRoutine = null;
        canAttack = false;
        canContinueAttack = false;
    }

    /// <summary>
    /// Spawns an instance of the given bullet pattern prefab as a child of the BulletHolder Singleton
    /// </summary>
    /// <param name="pattern"></param>
    public virtual void ShootPatternBullet(GameObject pattern) {
        GameObject pat = Instantiate(pattern, transform.position, Quaternion.identity, BulletHolder.Instance.transform);
        //if the pattern has an attached BulletPattern script and a ChangeBulletSpeed action was part of the current attack cycle index,
        //then update the speed on the BulletPattern script so it can update its bullet children
        BulletPattern bulPat;
        if (overridePatternSpeed && pat.TryGetComponent<BulletPattern>(out bulPat)) {
            bulPat.SetSpeedOverride(newPatternSpeed);
        }
    }

    /// <summary>
    /// Stops the enemy from continuing to go through their attack cycle until the given amount of time has passed
    /// </summary>
    /// <param name="time"></param>
    public void Wait(float time) {
        if (time == 0.0f) {
            return;
        }
        canContinueAttack = false;
        attackCycleRoutine = StartCoroutine(WaitForCycle(time));

        IEnumerator WaitForCycle(float time) {
            yield return new WaitForSeconds(time);
            //if the enemy can't attack (player isn't in range or its not on screen) don't bother letting it attack
            if (canAttack) {
                canContinueAttack = true;
            }
            //clear the routine so the next one can be stored
            attackCycleRoutine = null;
        }
    }

    /// <summary>
    /// Sets the flag for the enemy to change the bullet speed of any patterns shot after this action is called
    /// </summary>
    /// <param name="speed"></param>
    public void SetPatternSpeed(float speed) {
        overridePatternSpeed = true;
        newPatternSpeed = speed;
    }

    /// <summary>
    /// Deals damage to the enemy; if the enemy has 0 health it is disabled in the scene and attempts to spawn a health pickup
    /// </summary>
    /// <param name="amount"></param>
    public void Hurt(int amount) {
        if (isDamageable) {
            currentHealth -= amount;
            if (currentHealth <= 0) {
                gameObject.SetActive(false);
                if (healthDropChance > 0) {
                    SpawnPickup(healthDropChance);
                }
            }
        }
    }

    /// <summary>
    /// Spawns an instance of the health pickup prefab given to the enemy based on the enemy's drop change %
    /// </summary>
    /// <param name="chance"></param>
    public void SpawnPickup(float chance) {
        float randomNum = UnityEngine.Random.Range(0, 100);
        if (randomNum < chance) {
            Instantiate(healthDrop, transform.position, Quaternion.identity);
        }
    }
}