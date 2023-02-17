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

public abstract class Boss : MonoBehaviour, IDamageable {
    #region Health
    [SerializeField]
    private int maxHealth;
    protected int currentHealth;
    protected bool isDamageable = true;
    #endregion
    #region Attack
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
    protected GameObject pattern;
    protected int numberOfRepeats = 0;
    protected int indexToRepeat = 0;
    #endregion
    #region Node Movement
    protected Vector3 startPos;
    [SerializeField]
    protected bool usesNodeMovement = false;
    [SerializeField]
    protected List<Vector3> movementNodes;
    protected List<int> blacklistNodeIndices;
    [SerializeField]
    protected int nodeColumnCount = 1;
    [SerializeField]
    protected int nodeRowCount = 1;
    [SerializeField]
    protected Bounds nodeBounds;
    protected Vector2 nodeOffsets;
    protected int targetNodeIndex = -1;
    protected int currentNodeIndex = -1;
    protected float moveSpeed = 10.0f;
    #endregion

#if UNITY_EDITOR
    protected void OnDrawGizmos() {
        GUIContent content = new GUIContent($"Health: {currentHealth}");
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        Handles.Label(new Vector3(transform.position.x - 0.75f, transform.position.y + 1.0f, transform.position.z), content, style);

        if (usesNodeMovement) {
            Gizmos.color = Color.black;
            if (Application.isPlaying) {
                Gizmos.DrawWireCube(nodeBounds.center + startPos, nodeBounds.size);
            }
            else {
                Gizmos.DrawWireCube(nodeBounds.center + transform.position, nodeBounds.size);
            }
            Handles.color = Color.black;
            for (int i = 0; i < movementNodes.Count; i++) {
                Handles.DrawSolidDisc(movementNodes[i], Vector3.back, 0.2f);
            }
        }
    }
#endif

    protected void Start() {
        startPos = transform.position;
        canAttack = true;
        canContinueAttack = true;
        phasesList = new List<UnityEvent[]>();
        foreach (Phases phase in phasesStruct) {
            phasesList.Add(phase.attackCycle);
        }
        currentHealth = maxHealth;
        //if the boss uses node movement, populate the list of nodes based on the bounds defined in the inspector
        if (usesNodeMovement) {
            UpdateMovementNodePositions();
        }

        MoveToNewNode();
    }

    protected void Update() {
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

    [ContextMenu("Update Nodes")]
    protected void UpdateMovementNodePositions() {
        nodeOffsets = new Vector2(nodeBounds.size.x / (nodeColumnCount - 1), nodeBounds.size.y / (nodeRowCount - 1));
        movementNodes = new List<Vector3>();
        blacklistNodeIndices = new List<int>();
        for (int x = 0; x < nodeColumnCount; x++) {
            for (int y = 0; y < nodeRowCount; y++) {
                //x/y times the nodeOffset givess the location within the bounds the node should be and all the other stuff is used to calculaute the starting position
                movementNodes.Add(new Vector3(x * nodeOffsets.x + transform.position.x - nodeBounds.extents.x + nodeBounds.center.x,
                    y * nodeOffsets.y + transform.position.y - nodeBounds.extents.y + nodeBounds.center.y, 
                    1));
            }
        }
    }

    [ContextMenu("Start Attacking")]
    protected void StartAttackCycle() {
        canContinueAttack = true;
    }

    public virtual void ShootPatternBullet(GameObject pattern) {
        this.pattern = Instantiate(pattern, transform.position, Quaternion.identity, BulletHolder.Instance.transform);
        BulletPattern bulPat;
        if (this.pattern.TryGetComponent<BulletPattern>(out bulPat) && overridePatternSpeed) {
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
            if(ChangePhase()) {
                attackCycleIndex = 0;
                attackCycleRoutine = null;
                //numberOfRepeats = 1;
            }
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
        numberOfRepeats = UnityEngine.Random.Range(lower, higher+1);
        Debug.Log($"Repeating the next attack {numberOfRepeats} number of times");
    }

    public void Print() {
        Debug.Log("Finished Cycle");
    }

    [ContextMenu("Move to new Node")]
    public void MoveToNewNode() {
        targetNodeIndex = GetRandomNodeIndex();
        isDamageable = false;
        blacklistNodeIndices.Add(targetNodeIndex);
        canContinueAttack = false;
        attackCycleRoutine = StartCoroutine(MoveToTargetNode());

        IEnumerator MoveToTargetNode() {
            while (transform.position != movementNodes[targetNodeIndex]) {
                transform.position = Vector3.MoveTowards(transform.position, movementNodes[targetNodeIndex], moveSpeed * Time.deltaTime);
                yield return null;
            }
            blacklistNodeIndices.Remove(currentNodeIndex);
            currentNodeIndex = targetNodeIndex;
            targetNodeIndex = -1;
            isDamageable = true;
            attackCycleRoutine = null;
            canContinueAttack = true;
        }
    }

    /// <summary>
    /// generate a random index for the list of movement nodes and ensure that it is not on the blaclisted nodes
    /// </summary>
    /// <returns></returns>
    protected int GetRandomNodeIndex() {
        if(blacklistNodeIndices.Count == movementNodes.Count) {
            Debug.LogError($"Boss {gameObject.name} has filled their movement blacklist! Make sure to remove indicies from the blacklist when they are not needed.");
            return 0;
        }
        int newNodeIndex = UnityEngine.Random.Range(0, movementNodes.Count - blacklistNodeIndices.Count);
        while (blacklistNodeIndices.Contains(newNodeIndex)) {
            newNodeIndex++;
            if (newNodeIndex > movementNodes.Count) {
                newNodeIndex = 0;
            }
        }
        return newNodeIndex;
    }

    public abstract bool ChangePhase();
}
