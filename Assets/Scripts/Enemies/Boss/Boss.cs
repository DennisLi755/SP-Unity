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
    [SerializeField]
    private Sprite healthBarBorder;
    private BoxCollider2D hitbox;
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
    [SerializeField]
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
    [SerializeField]
    private GameObject afterImage;
    [SerializeField]
    protected Material whiteMaterial;
    private Material spritesDefault;
    protected SpriteRenderer sr;
    [SerializeField]
    GameObject[] walls;

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
            for (int i = 0; i < movementNodes.Count; i++) {
                Handles.color = new Color((float)i / movementNodes.Count, 0, 0);
                Handles.DrawSolidDisc(movementNodes[i], Vector3.back, 0.2f);
            }
        }
    }
#endif

    protected void Start() {
        //Set the health bar to its default state
        UIManager.Instance.SetBossHealthBarBorder(healthBarBorder);
        UIManager.Instance.UpdateBossHealthBar(1.0f);
        UIManager.Instance.EnableBossHealthBar(true);

        PlayerInfo.Instance.CombatLock = true;

        hitbox = transform.GetComponent<BoxCollider2D>();
        startPos = transform.position;
        canAttack = true;
        canContinueAttack = true;
        phasesList = new List<UnityEvent[]>();
        sr = GetComponent<SpriteRenderer>();
        spritesDefault = sr.material;
        //deconstruct the Phases struct into a list
        foreach (Phases phase in phasesStruct) {
            phasesList.Add(phase.attackCycle);
        }
        currentHealth = maxHealth;
        //if the boss uses node movement, populate the list of nodes based on the bounds defined in the inspector
        if (usesNodeMovement) {
            UpdateMovementNodePositions();
        }
        foreach (GameObject wall in walls) {
            wall.SetActive(true);
        }
        MoveToNewNode();
    }

    protected void Update() {
        if (canContinueAttack && phasesList[currentPhase].Length > 0) {
            overridePatternSpeed = false;
            phasesList[currentPhase][attackCycleIndex]?.Invoke();
            //check if the boss has already repeated the set index the number of times they need to,
            //or they are on a cycle command after the one they are supposed to repeat
            if (numberOfRepeats == 1 || indexToRepeat != attackCycleIndex) {
                attackCycleIndex++;
            }
            //otherwise count this as a completed repeat cycle
            else {
                numberOfRepeats--;
            }
            if (attackCycleIndex >= phasesList[currentPhase].Length) {
                attackCycleIndex = 0;
            }
        }
    }

    private void OnDestroy() {
        PlayerInfo.Instance.CombatLock = false;
        UIManager.Instance.EnableBossHealthBar(false);
    }

    /// <summary>
    /// Recaculates the placement of the boss's movement nodes; called once in Boss.Start(), 
    /// but could technically be called more if the row/colmn count or the size of the bounds gets changed during runtime (eg. phase change)
    /// </summary>
    [ContextMenu("Update Nodes")]
    protected void UpdateMovementNodePositions() {
        //the distance between each node
        nodeOffsets = new Vector2(nodeBounds.size.x / (nodeColumnCount - 1), nodeBounds.size.y / (nodeRowCount - 1));

        movementNodes = new List<Vector3>();
        blacklistNodeIndices = new List<int>();
        for (int y = nodeRowCount - 1; y >= 0; y--) {
            for (int x = 0; x < nodeColumnCount; x++) {
                //x/y times the nodeOffset givess the location within the bounds the node should be and all the other stuff is used to calculaute the starting position
                movementNodes.Add(new Vector3(x * nodeOffsets.x + transform.position.x - nodeBounds.extents.x + nodeBounds.center.x,
                    y * nodeOffsets.y + transform.position.y - nodeBounds.extents.y + nodeBounds.center.y,
                    1));
            }
        }
    }

    /// <inheritdoc cref="StaticEnemy.ShootPatternBullet(GameObject)"/>
    public virtual void ShootPatternBullet(GameObject pattern) {
        this.pattern = Instantiate(pattern, transform.position, Quaternion.identity, BulletHolder.Instance.transform);
        BulletPattern bulPat;
        if (this.pattern.TryGetComponent<BulletPattern>(out bulPat) && overridePatternSpeed) {
            bulPat.SetSpeedOverride(newPatternSpeed);
        }
    }

    /// <inheritdoc cref="StaticEnemy.Wait(float)"/>
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

    /// <inheritdoc cref="StaticEnemy.SetPatternSpeed(float)"/>
    public void SetPatternSpeed(float speed) {
        overridePatternSpeed = true;
        newPatternSpeed = speed;
    }

    /// <inheritdoc cref="StaticEnemy.Hurt(int)"/>
    public void Hurt(int amount) {
        if (isDamageable) {
            currentHealth -= amount;
            sr.material = whiteMaterial;
            StartCoroutine(TurnColorBack());
            if (currentHealth <= 0) {
                gameObject.SetActive(false);
                UIManager.Instance.EnableBossHealthBar(false);
                PlayerInfo.Instance.CombatLock = false;
                SoundManager.Instance.FadeOutCurrentLayer(3.0f);
                foreach (GameObject wall in walls) {
                    wall.SetActive(false);
                }
            }
            if (ChangePhase()) {
                attackCycleIndex = 0;
                attackCycleRoutine = null;
            }
            UIManager.Instance.UpdateBossHealthBar((float)currentHealth / maxHealth);
        }

        IEnumerator TurnColorBack() {
            yield return new WaitForSeconds(0.2f);
            sr.material = spritesDefault;
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
        numberOfRepeats = UnityEngine.Random.Range(lower, higher + 1);
        Debug.Log($"Repeating the next attack {numberOfRepeats} number of times");
    }

    /// <summary>
    /// Moves the boss to a random node from its movementNodes list
    /// </summary>
    [ContextMenu("Move to new Node")]
    public virtual void MoveToNewNode() {
        targetNodeIndex = GetRandomNodeIndex();
        isDamageable = false;
        blacklistNodeIndices.Add(targetNodeIndex);
        canContinueAttack = false;
        attackCycleRoutine = StartCoroutine(MoveToTargetNode());
        hitbox.enabled = false;

        ///Moves the boss to the target node and resets it relavent fields (eg. damageable, targetNodeIndex, etc) once it stops moving
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
            hitbox.enabled = true;
        }
    }

    /// <summary>
    /// generate a random index for the list of movement nodes and ensure that it is not on the blaclisted nodes
    /// </summary>
    /// <returns></returns>
    protected int GetRandomNodeIndex() {
        //the blacklist is filled with every indices, meaning there would be no valid node to go to, return 0 so the it does not crash and warn the dev
        if (blacklistNodeIndices.Count == movementNodes.Count) {
            Debug.LogError($"Boss {gameObject.name} has filled their movement blacklist! Make sure to remove indicies from the blacklist when they are not needed.");
            return 0;
        }

        //go through the relevant nodes to figure out which node indices are close nodes which have a lower chance of being chosen
        List<int> closeNodes = new List<int>();
        int cni = currentNodeIndex;
        //node to the right if there is one
        if (cni % nodeColumnCount != 0 && !blacklistNodeIndices.Contains(cni - 1)) {
            closeNodes.Add(cni - 1);
        }
        //node to the right if there is one
        if (cni + 1 % nodeColumnCount != 0 && !blacklistNodeIndices.Contains(cni + 1)) {
            closeNodes.Add(cni + 1);
        }
        //if there are more than 1 rows, get the nodes above/below 
        if (nodeRowCount > 1) {
            //nodes above if the current is on a row above the first
            if (cni >= nodeColumnCount) {
                if (!blacklistNodeIndices.Contains(cni - nodeColumnCount)) {
                    closeNodes.Add(cni - nodeColumnCount);
                }
                //nodes to the left and right of the node above
                if (cni % nodeColumnCount != 0 && !blacklistNodeIndices.Contains(cni - nodeColumnCount - 1)) {
                    closeNodes.Add(cni - nodeColumnCount - 1);
                }
                if (cni + 1 % nodeColumnCount != 0 && !blacklistNodeIndices.Contains(cni - nodeColumnCount + 1)) {
                    closeNodes.Add(cni - nodeColumnCount + 1);
                }
            }
            //nodes below if the current is on a row less than the last
            if (cni < movementNodes.Count - nodeColumnCount) {
                if (!blacklistNodeIndices.Contains(cni + nodeColumnCount)) {
                    closeNodes.Add(cni + nodeColumnCount);
                }
                //nodes to the left and right of the node above
                if (cni % nodeColumnCount != 0 && !blacklistNodeIndices.Contains(cni + nodeColumnCount - 1)) {
                    closeNodes.Add(cni + nodeColumnCount - 1);
                }
                if (cni + 1 % nodeColumnCount != 0 && !blacklistNodeIndices.Contains(cni + nodeColumnCount - 1)) {
                    closeNodes.Add(cni + nodeColumnCount + 1);
                }
            }
        }

        //effective nodes to the number of far nodes + 1 which consolidates every close node, because the current node is not considered a close node,
        //we can do the simple subtraction as the cni acts at the +1
        int effectiveNodes = movementNodes.Count - closeNodes.Count - blacklistNodeIndices.Count;
        //percent for each effective node
        float percentForEachNode = 1.0f / (effectiveNodes);
        float totalP = 0.0f;
        float ran = UnityEngine.Random.Range(0.0f, 1.0f);
        //go through all movement nodes, skipping blacklisted nodes, and add their percent to the total,
        //then check if the random number is below that total, if it is then that is the node to return
        for (int i = 0; i < movementNodes.Count; i++) {
            if (blacklistNodeIndices.Contains(i)) {
                continue;
            }
            totalP += closeNodes.Contains(i) ? percentForEachNode / closeNodes.Count : percentForEachNode;
            if (ran <= totalP) {
                return i;
            }
        }

        Debug.LogError($"The percent {ran} did not fall into any node's percent. Returning 0.");
        return 0;
    }

    /// <summary>
    /// Checks if the boss should change phase
    /// </summary>
    /// <returns></returns>
    public abstract bool ChangePhase();

    /// <summary>
    /// Creates Echo/After Images of the boss while moving
    /// </summary>
    /// <returns></returns>
    public IEnumerator CreateAfterImages() {
        while (targetNodeIndex != -1) {
            GameObject echoInstance = Instantiate(afterImage, transform.position, Quaternion.identity);
            echoInstance.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime * 3);
        }
    }
}