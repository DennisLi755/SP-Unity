using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInfo : MonoBehaviour {

    private static PlayerInfo instance;
    public static PlayerInfo Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<PlayerInfo>();
            }
            return instance;
        }
    }

    private PlayerControl pControl;
    public PlayerControl PlayerControl { get => pControl; }

    [SerializeField]
    private Sprite[] playerHitboxSprites;
    /// <summary>
    /// True for the standard health bar in the top left, false for the SP style 
    /// </summary>
    private bool healthBarStyle = true;

    private SpriteRenderer sr;

    #region Combat Control
    private bool inCombat = false;
    public bool InCombat => inCombat;
    private bool combatLock = false;
    public bool CombatLock {
        get => combatLock;
        set {
            combatLock = value;
            //ensure the enters combat when the lock is enabled
            if (combatLock) {
                EnterCombat();
            }
        }
    }
    public GameObject Hitbox => pControl.Hitbox;

    //Health
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    private const int startingHealth = 5;
    private const float invincibilityLength = 1;
    private bool damageable = true;
    public bool Damageable { get => damageable; set => damageable = value; }
    //Mana
    private int currentMana;
    public int CurrentMana {
        get => currentMana; set {
            currentMana = value;
            UIManager.Instance.UpdatePlayerMana((float)currentMana / startingMana);
        }
    }
    private const int startingMana = 100;

    [SerializeField]
    private bool attackUnlocked = false;
    public bool AttackUnlocked { get => attackUnlocked; set => attackUnlocked = value; }
    #endregion

    #region Object Interactions
    private bool canInteract;
    public bool CanInteract => canInteract;
    private InteractableObject interactable;
    public InteractableObject Interactable { get => interactable; set { interactable = value; } }
    #endregion

    private bool hasMoved = false;
    public bool HasMoved { get => hasMoved; set => hasMoved = value; }

    private void Awake() {
        if (instance == null) {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
        pControl = GetComponent<PlayerControl>();
    }

    private void Start() {
        sr = GetComponent<SpriteRenderer>();

        currentHealth = startingHealth;
        currentMana = startingMana;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            FlipHealthBarStyle();
        }
    }

    /// <summary>
    /// Damages the player the given amount and starts their I-Frames
    /// </summary>
    /// <param name="amount"></param>
    public void Hurt(int amount) {
        //if the player is not currently damageable (most likely from I-Frames), do nothing
        if (!damageable) {
            return;
        }
        damageable = false;
        SoundManager.Instance.PlaySoundEffect("PlayerHurt", SoundSource.player);
        if (!pControl.IsShieldActive) {
            currentHealth -= amount;
        }
        else {
            pControl.ToggleShield(false);
        }
        if (healthBarStyle) {
            UIManager.Instance.UpdatePlayerHealth((float)currentHealth / startingHealth);
        }
        else {
            UpdateHitboxHealth();
        }
        //Player is dead
        if (currentHealth <= 0) {
            sr.color = Color.red;
            pControl.Velocity = Vector3.zero;
            pControl.CanMove = false;
            damageable = false;
            GameManager.Instance.EndFight();
        }
        //otherwise give them I-Frames
        else {
            StartCoroutine(WaitForIFrames());
        }

        ///Wait for a total length of the player's invinicibility length, changing the opacity of the player's sprite a 
        ///number of times set inside the routine
        IEnumerator WaitForIFrames() {
            const int flashCount = 7;
            bool fullOpacity = false;
            for (int i = 0; i < flashCount; i++, fullOpacity = !fullOpacity) {
                //player's sprite opacity
                Color newColor = sr.color;
                newColor.a = fullOpacity ? 1.0f : 0.5f;
                sr.color = newColor;

                //hitbox color
                Hitbox.GetComponent<SpriteRenderer>().color = fullOpacity? Color.white : Color.red;
                yield return new WaitForSeconds(invincibilityLength / flashCount);
            }
            //ensure that when I-Frames end the player is at full-opacity
            Color fullOpa = sr.color;
            fullOpa.a = 1.0f;
            sr.color = fullOpa;
            Hitbox.GetComponent<SpriteRenderer>().color = Color.white;
            damageable = true;
        }
    }

    /// <summary>
    /// Heals the player the given amount without going over their starting health
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount) {
        if (currentHealth < startingHealth) {
            currentHealth += amount;
        }
        if (healthBarStyle) {
            UIManager.Instance.UpdatePlayerHealth((float)currentHealth / startingHealth);
        }
        else {
            UpdateHitboxHealth();
        }
    }

    public void Respawn() {
        currentHealth = startingHealth;
        UIManager.Instance.UpdatePlayerHealth((float)currentHealth/startingHealth);
        sr.color = Color.white;
        pControl.CanMove = true;
        damageable = true;
    }

    /// <summary>
    /// Sets the flags for the player being able to interact and tracking which game object would interacted with
    /// </summary>
    /// <param name="sender"></param>
    public void EnterInteractable(InteractableObject sender) {
        canInteract = true;
        interactable = sender;
    }

    /// <summary>
    /// Removes the flags of the player being able interact, only if the sender is the currently tracked interactable object
    /// </summary>
    /// <param name="sender"></param>
    public void ExitInteractable(InteractableObject sender) {
        if (interactable == sender) {
            canInteract = false;
            interactable = null;
        }
    }

    /// <summary>
    /// Changes the input mapping of the PlayerInput component; used to swap between UI and world navigation
    /// </summary>
    /// <param name="map"></param>
    public void ChangeInputMap(string map) {
        GetComponent<PlayerInput>().SwitchCurrentActionMap(map);
    }

    /// <summary>
    /// Removes the player from combat, disabling their hitbox and any other combat related UI or restrictions
    /// </summary>
    public void ExitCombat() {
        //if the player is locked in combat do nothing; this is most likely caused by a boss currently being active, or a minigame that requires the player to be in combat the entire time
        if (combatLock) {
            return;
        }
        inCombat = false;
        Hitbox.SetActive(false);
        UIManager.Instance.EnablePlayerHealthBar(false);
        //UIManager.Instance.EnablePlayerManaBar(false);
        UIManager.Instance.EnableSkillIcons(false);
    }

    /// <summary>
    /// Enteres the player into combat, enabling their hitbox and any other combat related UI or restrictions
    /// </summary>
    public void EnterCombat() {
        inCombat = true;
        Hitbox.SetActive(true);
        UIManager.Instance.EnablePlayerHealthBar(true);
        //UIManager.Instance.EnablePlayerManaBar(true);
        UIManager.Instance.EnableSkillIcons(true);
    }

    public void UpdateHitboxHealth() {
        Hitbox.GetComponent<SpriteRenderer>().sprite = playerHitboxSprites[currentHealth];
    }

    public void FlipHealthBarStyle() {
        //changing from standard type
        if (healthBarStyle) {
            Hitbox.GetComponent<SpriteRenderer>().sprite = playerHitboxSprites[currentHealth];
            UIManager.Instance.EnablePlayerHealthBar(false);
        }
        //changing from SP style
        else {
            Hitbox.GetComponent<SpriteRenderer>().sprite = playerHitboxSprites[6];
            UIManager.Instance.EnablePlayerHealthBar(true);
            UIManager.Instance.UpdatePlayerHealth((float)currentHealth / startingHealth);
        }
        healthBarStyle = !healthBarStyle;
    }

    public void EnableTutorialText(string text) {
        transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = text;
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void DisableTutorialText() {
        transform.GetChild(2).gameObject.SetActive(false);
    }
}