using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The position origins of the raycasts used to keep the player in bounds
/// </summary>
public struct RayCastOrigins {
    public Vector2 topLeft, topRight, bottomLeft, bottomRight;
}

/// <summary>
/// Which directions the player is colliding with a solid collider
/// </summary>
public struct CollisionDirections {
    public bool up, down, right, left;
    public void Reset() {
        up = down = right = left = false;
    }
}

[Serializable]
public struct AttackHitbox {
    public bool showBounds;
    public FacingDirection direction;
    public Bounds bounds;
}

/// <summary>
/// Which direction the player is facing; used to determine if they can interact with an object
/// </summary>
public enum FacingDirection {
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3
}

/// <summary>
/// The possible states the player can be in
/// </summary>
public enum PlayerState {
    Idle,
    Attack,
    Dashing
}

public enum SkillEquipStatus {
    NotUnlocked = -1,
    Unequipped = 0,
    Equipped = 1,
    Swapped = 2
}

public class PlayerControl : MonoBehaviour {

    [SerializeField]
    private bool showDebug = true;

    private Animator animator;
    [SerializeField]
    private GameObject afterImage;
    private PlayerInfo pInfo;

    private PlayerState playerState = PlayerState.Idle;

    #region Movement
    //movement
    private bool canMove = true;
    public bool CanMove { get => canMove; set { canMove = value; } }
    private const float walkSpeed = 5f;
    //We use a separate input vector to be able to continously collect the player's input, even if movement is disabled
    private Vector2 input;
    private Vector2 velocity;
    public Vector2 Velocity { get => velocity; set => velocity = value; }
    private float focusScalar = 1f;
    private FacingDirection facingDirection = FacingDirection.Down;

    //dashing
    private bool canDash = true;
    private const int maxDashCharges = 2;
    private const float dashSpeed = 20f;
    private int currentDashCharges = maxDashCharges;
    private float dashImmunityLength = 0.24f;
    //how long the player is dashing for in seconds
    private float dashLength = 0.12f;
    //how long it takes for a dash charge to come back, starting when the dash ends
    private float dashRechargeTime = 1.5f;

    //active move speed is changed between the value of walkSpeed and dashSpeed when the player starts or ends a dash
    //by default is is equal to walkSpeed
    private float activeMoveSpeed = walkSpeed;
    #endregion

    #region Attack Info
    LayerMask attackableLayers;
    [SerializeField]
    private AnimationClip attackAnimation;
    [SerializeField]
    private int damageFrame;
    private int totalAttackFrames;
    private GameObject hitbox;
    public GameObject Hitbox => hitbox;

    public bool AttackUnlocked => pInfo.AttackUnlocked;
    private float attackMoveSpeed = 3f;
    [SerializeField]
    private AttackHitbox[] inspectorAttackHixboxes = new AttackHitbox[4];
    private Dictionary<FacingDirection, Bounds> attackHitboxes = new Dictionary<FacingDirection, Bounds>();
    #endregion

    #region Raycasting & Collisions
    //the layers that the player will collide with the environment
    private LayerMask environmentLayers;
    private new BoxCollider2D collider;
    private LayerMask bulletLayer;
    private CircleCollider2D hitboxCollider;
    private Bounds bounds => collider.bounds;
    private CollisionDirections collisionDirs;
    //The inner corners of the player's hitbox that raycasts start from
    private RayCastOrigins rayOrigins;
    const float skinWidth = 0.015f;
    const int horizontalRayCount = 3;
    const int verticalRayCount = 3;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    public FacingDirection FacingDirection => facingDirection;

    #endregion

    #region Skills
    [SerializeField]
    private Skills skillsCollection;
    private int[] equippedSkills = new int[2] { -1, -1 };
    public int[] EquippedSkills => equippedSkills;
    private float[] skillsCooldowns = new float[] { 0.0f, 0.0f };
    private List<int> unlockedSkills = new List<int>();
    public int[] UnlockedSkills => unlockedSkills.ToArray();
    private List<Func<bool>> allSkills;

    //skill 0
    GameObject shield;
    private bool isShieldActive = false;
    public bool IsShieldActive => isShieldActive;
    #endregion

#if UNITY_EDITOR
    #region Editor UI
    private void OnDrawGizmos() {
        //draw the player's attack boxes
        Gizmos.color = Color.red;
        foreach (AttackHitbox hitbox in inspectorAttackHixboxes) {
            if (hitbox.showBounds) {
                Gizmos.DrawWireCube(hitbox.bounds.center + transform.position, hitbox.bounds.size);
            }
        }
    }

    /// <summary>
    /// Draws debug info to the screen
    /// </summary>
    private void OnGUI() {
        if (showDebug && !GameManager.Instance.IsPaused) {
            GUI.color = Color.black;
            int yStart = Screen.height / 8;
            GUI.Box(new Rect(0, yStart + 15, 150, 200), "");
            GUI.color = Color.white;
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"activeMoveSpeed: {activeMoveSpeed}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"dashCharges: {currentDashCharges}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"playerState: {playerState}");

            GUI.Label(new Rect(5, yStart += 30, 300, 150), $"up collision: {collisionDirs.up}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"right collision: {collisionDirs.right}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"down collision: {collisionDirs.down}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"left collision: {collisionDirs.left}");

            GUI.Label(new Rect(5, yStart += 30, 300, 150), $"facing direction: {facingDirection}");

            GUI.Label(new Rect(5, yStart += 30, 300, 150), $"health: {PlayerInfo.Instance.CurrentHealth}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"mana: {PlayerInfo.Instance.CurrentMana}");

            GUI.Label(new Rect(5, yStart += 30, 300, 150), $"skill in slot 0: {equippedSkills[0]}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"skill slot 0 cooldown: {skillsCooldowns[0]}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"skill in slot 1: {equippedSkills[1]}");
            GUI.Label(new Rect(5, yStart += 15, 300, 150), $"skill slot 1 cooldown: {skillsCooldowns[1]}");
        }
    }
    #endregion
#endif

    /// <summary>
    /// Runs when the object is enabled for the first time
    /// </summary>
    void Start() {
        pInfo = GetComponent<PlayerInfo>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        hitbox = transform.GetChild(0).gameObject;
        shield = transform.GetChild(1).gameObject;
        hitboxCollider = hitbox.GetComponent<CircleCollider2D>();

        collisionDirs = new CollisionDirections();
        rayOrigins = new RayCastOrigins();
        CalculateRaySpacing();
        UpdateRayCastOrigins();

        bulletLayer = LayerMask.GetMask("Bullet");
        environmentLayers = LayerMask.GetMask("Environment", "Enemy");
        attackableLayers = LayerMask.GetMask("Enemy", "Bullet");

        totalAttackFrames = (int)(attackAnimation.length * attackAnimation.frameRate);
        foreach (AttackHitbox hitbox in inspectorAttackHixboxes) {
            attackHitboxes.Add(hitbox.direction, hitbox.bounds);
        }

        allSkills = new List<Func<bool>>() {
            Skill0,
            Skill1,
            Skill2,
            Skill3
        };
        //SetupSkills();
        UnlockSkill(0);
    }

    /// <summary>
    /// Runs every frame (variable depending on hardware)
    /// </summary>
    void Update() {
        switch (playerState) {
            case PlayerState.Idle:
                if (velocity != Vector2.zero) {
                    animator.SetFloat("Horizontal", velocity.x);
                    animator.SetFloat("Vertical", velocity.y);
                }
                animator.SetFloat("Speed", velocity.sqrMagnitude);

                //sets the player's facing direction based on their velocity
                if (velocity.y > 0)
                    facingDirection = FacingDirection.Up;
                else if (velocity.y < 0)
                    facingDirection = FacingDirection.Down;
                else if (velocity.x > 0)
                    facingDirection = FacingDirection.Right;
                else if (velocity.x < 0)
                    facingDirection = FacingDirection.Left;
                break;
        }
    }

    /// <summary>
    /// Runs every physics frame (default is 50 times a second)
    /// </summary>
    void FixedUpdate() {
        //stops the player when they run into a wall, leave commented if you want the player to continue their run animation against a wall
        /*if (collisionDirs.down || collisionDirs.up) {
            velocity.y = 0;
        }
        if (collisionDirs.left || collisionDirs.right) {
            velocity.x = 0;
        }*/

        UpdateRayCastOrigins();
        collisionDirs.Reset();

        //put any scalar values before velocity for better performance
        Vector2 newVel = activeMoveSpeed * focusScalar * Time.fixedDeltaTime * velocity;

        //if the player is moving vertically or horizontally, check for collisions in those directions
        if (velocity.x != 0) {
            HorizontalCollisions(ref newVel);
        }
        if (velocity.y != 0) {
            VerticalCollisions(ref newVel);
        }

        //check the player's hitbox for any bullets
        if (pInfo.InCombat) {
            RaycastHit2D hit = Physics2D.CircleCast(hitboxCollider.bounds.center, hitboxCollider.radius, Vector2.zero, 0.0f, bulletLayer);
            if (hit) {
                pInfo.Hurt(1);
            }

            /*hit = Physics2D.BoxCast(bounds.center, bounds.size, 0.0f, Vector2.zero, 0.0f, attackableLayers);
            if (hit) {
                Vector2 dir = bounds.center - hit.collider.bounds.center;
                hit = Physics2D.Raycast(bounds.center, dir.normalized, 1.0f, attackableLayers);
                Vector2 correction = dir * hit.fraction;
            }*/
        }

        //move the player by the (possibly) corrected velocity
        transform.Translate(newVel);
    }

    #region Collisions & Raycasts
    /// <summary>
    /// Calculates the spacing of the raycasts used for player collision
    /// This is only run once: at startup
    /// </summary>
    private void CalculateRaySpacing() {
        //We make a copy of the bounds so that we can expand it without affect the player's actual hitbox
        Bounds tempBounds = bounds;
        tempBounds.Expand(skinWidth * -2);

        horizontalRaySpacing = tempBounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = tempBounds.size.x / (verticalRayCount - 1);
    }

    /// <summary>
    /// Updates the world position of the raycasts in the 
    /// </summary>
    private void UpdateRayCastOrigins() {
        Bounds tempBounds = bounds;
        tempBounds.Expand(skinWidth * -2);

        rayOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        rayOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    /// <summary>
    /// Determines if the player will collide with an object in the upleft/right direction
    /// If a collision were to take place, adjusts the player's velocity so that they will not
    /// </summary>
    /// <param name="velocity">reference to the player's current velocity</param>
    private void HorizontalCollisions(ref Vector2 velocity) {
        //left is negative, right is positive
        float directionX = Mathf.Sign(velocity.x);
        //length of the ray is scaled by the player's velocity plus the skinWidth
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
            //depending on which way the player is moving, change the starting corner to raycast from
            Vector2 rayOrigin = (directionX == -1) ? rayOrigins.bottomLeft : rayOrigins.bottomRight;
            //adjust the starting position based on iteration
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, environmentLayers);

            //if the ray cast hits, change the velocity so the player will not end up in the object and set the collision direction
            if (rayHit) {
                velocity.x = (rayHit.distance - skinWidth) * directionX;
                //prevents rays past the first one from override the velocity adjustment if they would hit
                rayLength = rayHit.distance;

                //set collision direction based on movement direction
                collisionDirs.right = !(collisionDirs.left = directionX == -1);
            }
            Debug.DrawRay(rayOrigin, directionX * rayLength * Vector2.right, Color.blue);
        }
    }

    /// <summary>
    /// Determines if the player will collide with an object in the up/down direction
    /// If a collision were to take place, adjusts the player's velocity so that they will not
    /// </summary>
    /// <param name="velocity">reference to the player's current velocity</param>
    private void VerticalCollisions(ref Vector2 velocity) {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1) ? rayOrigins.bottomLeft : rayOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, environmentLayers);

            if (rayHit) {
                velocity.y = (rayHit.distance - skinWidth) * directionY;
                //prevents rays past the first one from override the velocity adjustment if they would hit
                rayLength = rayHit.distance;

                //set collision direction based on movement direction
                collisionDirs.up = !(collisionDirs.down = directionY == -1);
            }

            Debug.DrawRay(rayOrigin, directionY * rayLength * Vector2.up, Color.blue);
        }
    }
    #endregion

    #region Input
    #region Movement
    /// <summary>
    /// Interprets the player's input for movement
    /// </summary>
    /// <param name="context"></param>
    public void Move(InputAction.CallbackContext context) {
        input = context.ReadValue<Vector2>().normalized;
        if (canMove) {
            velocity = input;
        }
    }
    #endregion

    #region Interact

    public void Interact(InputAction.CallbackContext context) {
        if (context.performed && PlayerInfo.Instance.CanInteract) {
            PlayerInfo.Instance.Interactable.OnInteract();
        }
    }

    #endregion

    #region Attack
    /// <summary>
    /// Interprets the player's input for attacking
    /// </summary>
    /// <param name="context"></param>
    public void Attack(InputAction.CallbackContext context) {
        if (context.performed && playerState != PlayerState.Attack && AttackUnlocked) {
            SoundManager.Instance.PlaySoundEffect("PlayerAttack", SoundSource.player);
            animator.SetBool("IsAttacking", true);
            playerState = PlayerState.Attack;
            //ensures the player is set to walking speed if they attack cancel a dash
            activeMoveSpeed = attackMoveSpeed;
            //disables dashing so the player cannot dash cancel until the attack boxcast has been done
            //and disables movement so the player cannot change their direction
            canDash = false;
            canMove = false;
            StartCoroutine(EndAttack());
        }
    }

    /// <summary>
    /// Waits for the damage frame of the attack animation then re-enables dashing and performs a boxcast for any enemies in front of the player
    /// After the full length of the attack animation sets the player state back to idle
    /// </summary>
    /// <returns></returns>
    IEnumerator EndAttack() {
        yield return new WaitForSeconds((float)damageFrame / totalAttackFrames * attackAnimation.length);
        #region Boxcasting for attackables
        Bounds hitBoxBounds = attackHitboxes[facingDirection];
        //get every hit object
        RaycastHit2D[] attackHits = Physics2D.BoxCastAll(hitBoxBounds.center + transform.position, hitBoxBounds.size, 0.0f, Vector2.zero, 0.0f, attackableLayers);
        //if there were any hits, resolve them
        if (attackHits.Length > 0) {
            foreach (RaycastHit2D hit in attackHits) {
                GameObject hitObject = hit.transform.gameObject;
                //resolving depends on layer (or further on tag)
                switch (LayerMask.LayerToName(hitObject.layer)) {
                    case "Enemy":
                        hitObject.GetComponent<IDamageable>().Hurt(1);
                        break;
                    case "Bullet":
                        Bullet bullet = hitObject.GetComponent<Bullet>();
                        if (bullet.BulletType == BulletType.Deflectable) {
                            if (InDirectionRange(bullet.Angle)) {
                                bullet.Direction = -bullet.Direction;
                            } else {
                                Vector2 direction = new Vector2(facingDirection == FacingDirection.Right ? 1 : facingDirection == FacingDirection.Left ? -1 : 0, 
                                facingDirection == FacingDirection.Up ? 1 : facingDirection == FacingDirection.Down ? -1 : 0);
                                bullet.Direction = direction;
                            }
                            bullet.Speed *= 2;
                        }
                        break;
                    default:
                        Debug.LogError($"Player attack has not been setup for layer: {LayerMask.LayerToName(hit.transform.gameObject.layer)}" +
                            $"\nThis was actived by hitting {hitObject} at {hitObject.transform.position}" +
                            $"\nThis layer might have been added to the player's attackable layers by mistake.");
                        break;
                }
            }
        }
        #endregion

        //enable dashing 
        canDash = true;

        //end the attack state
        yield return new WaitForSeconds((1.0f - (float)damageFrame / totalAttackFrames) * attackAnimation.length);
        if (playerState != PlayerState.Dashing) {
            playerState = PlayerState.Idle;
            activeMoveSpeed = walkSpeed;
        }
        animator.SetBool("IsAttacking", false);
        canMove = true;
        velocity = input;
    }

    public bool InDirectionRange(float angle) {
        float[] currentDirection = new float[2];
        switch (facingDirection) {
            case FacingDirection.Up:
                currentDirection[0] = 5;
                break;
            case FacingDirection.Down:
                currentDirection[0] = 185;
                break;
            case FacingDirection.Left:
                currentDirection[0] = 95;
                break;
            case FacingDirection.Right:
                currentDirection[0] = 275;
                break;
        }
        currentDirection[1] = currentDirection[0] + 175;
        if (angle+180 > currentDirection[0] && angle+180 < currentDirection[1]) {
            return true;
        } else {
            return false;
        }
    }

    #endregion

    #region Dash
    /// <summary>
    /// Interprets the player's input for dashing
    /// </summary>
    /// <param name="context"></param>
    public void Dash(InputAction.CallbackContext context) {
        if (context.performed && currentDashCharges > 0 && playerState != PlayerState.Dashing && canDash && velocity != Vector2.zero) {
            SoundManager.Instance.PlaySoundEffect("PlayerDash", SoundSource.player);
            //setting canMove to true so that they can change dash directions if they dash cancel an attack
            canMove = true;
            velocity = input;
            playerState = PlayerState.Dashing;
            PlayerInfo.Instance.Damageable = false;
            activeMoveSpeed = dashSpeed;
            currentDashCharges--;
            StartCoroutine(EndDash());
            StartCoroutine(CreateAfterImages());
        }
    }

    /// <summary>
    /// Ends the player's dash after the set time by setting their state to idle and setting the activeMoveSpeed to walkSpeed
    /// Waits the set amount of time before recharging the used dash charge
    /// </summary>
    /// <returns></returns>
    IEnumerator EndDash() {
        yield return new WaitForSeconds(dashLength);
        //if the player ends dashing in the dash state, change back to idle state
        if (playerState == PlayerState.Dashing) {
            playerState = PlayerState.Idle;
            activeMoveSpeed = walkSpeed;
            //if the player ends dashing in the attack state, keep the reduced attack speed
        }
        else if (playerState == PlayerState.Attack) {
            activeMoveSpeed = attackMoveSpeed;
        }

        yield return new WaitForSeconds(dashImmunityLength);
        PlayerInfo.Instance.Damageable = true;

        yield return new WaitForSeconds(dashRechargeTime - dashImmunityLength);
        currentDashCharges++;
    }

    /// <summary>
    /// Creates the afterimage clones of the player while dashing
    /// Every other physics frame a clone is created
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateAfterImages() {
        while (playerState == PlayerState.Dashing) {
            GameObject echoInstance = Instantiate(afterImage, transform.position, Quaternion.identity);
            echoInstance.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime * 2);
        }
    }
    #endregion

    #region Focus
    /// <summary>
    /// Interprets the player's input for focus mode
    /// </summary>
    /// <param name="context"></param>
    public void Focus(InputAction.CallbackContext context) {
        if (context.performed) {
            focusScalar = 0.7f;
        }
        else if (context.canceled) {
            focusScalar = 1f;
        }
    }
    #endregion

    #region Skill Input
    public void ActivateSkill(InputAction.CallbackContext context) {
        if (context.performed) {
            //parse the input action to determine which skill to activate
            //we subtract '1' instead of '0' because we want it be 0 based for indexing the array
            int skillSlot = context.action.name[^1] - '1';
            int skillID = equippedSkills[skillSlot];
            //if the skill in the used slot is not been set, then doing nothing
            if (skillID == -1) {
                Debug.LogWarning($"There is no skill equipped in slot {skillSlot}");
                return;
            }

            //only activate the skill if its not on cooldown
            if (skillsCooldowns[skillSlot] == 0.0f) {
                if (pInfo.CurrentMana >= skillsCollection.skills[skillID].ManaCost) {
                    //activate the skill and only set the cooldown if the skill successfully activated
                    if (allSkills[skillID]()) {
                        //pInfo.CurrentMana -= skillsCollection.skills[skillIndex].ManaCost;
                    }
                }
                //player doesn't have enough mana
                else {
                Debug.Log($"Could not activate skill equipped in slot {skillSlot}: you do not have enough mana for that skill");
                }
            }
            //skill is still on cooldown
            else {
                Debug.Log($"Could not activate skill equipped in slot {skillSlot}: that skill is still on cooldown");
            }
        }
    }

    public IEnumerator CooldownSkill(int skillSlot, Func<bool> startCondition) {
        int skillID = equippedSkills[skillSlot];
        //set the cooldown for the skill in the same index as the skillSlot using the cooldown from the skills array with the index = skillID of the activated skill
        skillsCooldowns[skillSlot] = skillsCollection.skills[skillID].Cooldown;
        float maxSkillCooldown = skillsCollection.skills[skillID].Cooldown;
        while (startCondition()) {
            yield return null;
        }
        //do the cooldown overtime instead of just a WaitForSeconds in case we to have a visual indicator of how long the cooldown has left
        while (skillsCooldowns[skillSlot] > 0.0f) {
            skillsCooldowns[skillSlot] -= Time.deltaTime;
            UIManager.Instance.UpdateSkillIconCooldown(skillSlot, skillsCooldowns[skillSlot] / maxSkillCooldown);
            yield return null;
        }
        skillsCooldowns[skillSlot] = 0.0f;
        UIManager.Instance.UpdateSkillIconCooldown(skillSlot, 0.0f);
    }
    #endregion

    #region Open Menu
    public void ToggleMenu(InputAction.CallbackContext context) {
        if (context.performed) {
            GameManager.Instance.PauseGame();
        }
    }
    #endregion
    #endregion

    #region Skills Logic
    /// <summary>
    /// Tries to equip the skill with the given ID in the given slot 
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="skillSlot"></param>
    /// <returns></returns>
    public SkillEquipStatus EquipSkill(int skillID, int skillSlot) {
        //check if the skill is already equipped in the given slot; if it is, unequip it
        if (skillID == equippedSkills[skillSlot]) {
            Debug.Log($"Unequipped skill {skillID} from slot {skillSlot}");
            equippedSkills[skillSlot] = -1;
            return SkillEquipStatus.Unequipped;
        }
        //if the skill is equipped in the other skill slot, then unequip it from there and equip it in the new slot
        //The absolute value calculation is to make sure that the value is either 0 or 1 to get the other slot:
        // skill slot trying to equip to
        // |                     other skill slot
        //\/                    \/ 
        // 1 - 1 = 0    | 0 | = 0
        // 0 - 1 = -1   |-1 | = 1
        int otherSlot = Math.Abs(skillSlot - 1);
        if (skillID == equippedSkills[otherSlot]) {
            Debug.Log($"Unequipped skill {skillID} from slot {otherSlot} and equipped it to slot ${skillSlot}");
            equippedSkills[otherSlot] = -1;
            equippedSkills[skillSlot] = skillID;
            return SkillEquipStatus.Swapped;
        }
        //check if the skill is unlocked, if it isn't 
        if (!unlockedSkills.Contains(skillID)) {
            Debug.LogWarning($"Could not equip skill {skillID}, the player does not have that skill Unlocked");
            return SkillEquipStatus.NotUnlocked;
        }
        //player can successfull equip skill
        equippedSkills[skillSlot] = skillID;
        Debug.Log($"Skill {skillID} was equipped in slot {skillSlot}");
        return SkillEquipStatus.Equipped;
    }

    /// <summary>
    /// Adds a skill to the list of the player's unlocked skills
    /// </summary>
    /// <param name="skillID">The ID number of the skill to unlock</param>
    public void UnlockSkill(int skillID) {
        //only add the skillID if it has already not been unlocked
        if (!unlockedSkills.Contains(skillID)) {
            unlockedSkills.Add(skillID);
        }
    }

    private bool Skill0() {
        int skillSlot = (equippedSkills[0] == 0)? 0 : 1;
        if (!IsShieldActive) {
            ToggleShield(true);
            StartCoroutine(CooldownSkill(skillSlot, () => isShieldActive));
            return true;
        }
        else {
            Debug.Log("Skill 0 not activated; you already have a shield!");
            return false;
        }
    }

    public void ToggleShield(bool enabled) {
        isShieldActive = enabled;
        //active shield
        shield.SetActive(isShieldActive);
    }

    private bool Skill1() {
        Debug.Log("This is Skill1");
        return true;
    }

    private bool Skill2() {
        Debug.Log("This is Skill2");
        return true;
    }

    private bool Skill3() {
        Debug.Log("This is Skill3");
        return true;
    }

    ///Skill Testing Helpers

    [ContextMenu("Equip Skill 2")]
    public void EquipSkill2() {
        EquipSkill(2, 0);
    }

    private void SetupSkills() {
        UnlockSkill(0);
        UnlockSkill(2);
    }
    #endregion

    #region External Controls
    /// <summary>
    /// Zeros the player's current velocity and disables their movement
    /// </summary>
    public void Freeze() {
        canMove = false;
        velocity = Vector2.zero;
    }

    /// <summary>
    /// Enables the player's movement and sets velocity to the current input vector
    /// </summary>
    public void UnFreeze() {
        canMove = true;
        velocity = input;
    }

    /// <summary>
    /// Forces the player to face a certain direction; used for scripted events when you want the player to change directions without moving
    /// </summary>
    /// <param name="direction"></param>
    public void ForceDirection(FacingDirection direction) {
        facingDirection = direction;
        Vector2 dir = new Vector2();
        switch (direction) {
            case FacingDirection.Up:
                dir = new Vector2(0, 1);
                break;
            case FacingDirection.Right:
                dir = new Vector2(1, 0);
                break;
            case FacingDirection.Down:
                dir = new Vector2(0, -1);
                break;
            case FacingDirection.Left:
                dir = new Vector2(-1, 0);
                break;
        }
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
    }
    #endregion
}