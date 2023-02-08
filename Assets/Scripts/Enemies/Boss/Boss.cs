using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
[Serializable]
public struct Phases {
    public UnityEvent[] attackCycle;
}

public class Boss : MonoBehaviour
{
    #region Health
    [SerializeField]
    private int maxHealth;
    private int currentHealth;
    #endregion
    [SerializeField]
    protected Phases[] phasesStruct;
    [SerializeField]
    protected List<UnityEvent[]> phasesList;
    protected bool canAttack = false;
    protected bool canContinueAttack = false;
    protected int attackCycleIndex = 0;
    protected Coroutine attackCycleRoutine;
    protected bool overridePatternSpeed = false;
    protected float newPatternSpeed;
    protected int currentPhase = 0;
    protected GameObject pat;
    protected int numberOfRepeats = 0;
    protected int indexToRepeat = 0;

    protected void OnDrawGizmos() {
        GUIContent content = new GUIContent($"Health: {currentHealth}");
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        Handles.Label(new Vector3(transform.position.x - 0.75f, transform.position.y + 1.0f, transform.position.z), content, style);
    }

    protected void Start() {
        if (GetComponent<SpriteRenderer>().isVisible) {
            canAttack = true;
        }
        phasesList = new List<UnityEvent[]>();
        foreach (Phases phase in phasesStruct) {
            phasesList.Add(phase.attackCycle);
        }
        currentHealth = maxHealth;
    }

    protected void Update() {
        if (!canAttack && GetComponent<SpriteRenderer>().isVisible) {
            canAttack = true;
            canContinueAttack = true;
        }
        else if (canAttack && !GetComponent<SpriteRenderer>().isVisible) {
            canAttack = false;
        }

        if (canContinueAttack && phasesList[currentPhase].Length > 0) {
            overridePatternSpeed = false;
            phasesList[currentPhase][attackCycleIndex]?.Invoke();
            if (numberOfRepeats == 1 || indexToRepeat != attackCycleIndex) {
                attackCycleIndex++;
            } else {
                numberOfRepeats--;
            }
            if (attackCycleIndex >= phasesList[currentPhase].Length) {
                attackCycleIndex = 0;
            }
        }
    }

    [ContextMenu("Start Attacking")]
    protected void StartAttackCycle() {
        canContinueAttack = true;
    }

    public virtual void ShootPatternBullet(GameObject pattern) {
        pat = Instantiate(pattern, transform.position, Quaternion.identity, BulletHolder.Instance.transform);
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
    /// <summary>
    /// Make a specific event fire an amount of times
    /// </summary>
    /// <param name="rand">"lower number,higher number, index of event to repeat"</param>
    public void RepeatEventRandomTimes(string rand) {
        string[] numString = rand.Split(",");
        int lower = int.Parse(numString[0]);
        int higher = int.Parse(numString[1]);
        indexToRepeat = int.Parse(numString[2]);
        System.Random rng = new System.Random();
        numberOfRepeats = rng.Next(lower, higher+1);
        Debug.Log(numberOfRepeats);
    }

    public void Print() {
        Debug.Log("HIHIIHII");
    }

    public virtual void ChangePhase() {}
    //movement?
}
