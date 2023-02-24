using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    private float health;
    public float Health => health;
    private const float startingHealth = 5;
    private const float invincibilityLength = 1;
    private bool damageable = true;
    public bool Damageable { get => damageable; set { damageable = value; } }

    [SerializeField]
    private bool attackUnlocked = true;
    public bool AttackUnlocked { get => attackUnlocked; set => attackUnlocked = value; }
    #endregion

    #region Object Interactions
    private bool canInteract;
    public bool CanInteract => canInteract;
    private InteractableObject interactable;
    public InteractableObject Interactable { get => interactable; set { interactable = value; } }
    #endregion

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        pControl = GetComponent<PlayerControl>();
        sr = GetComponent<SpriteRenderer>();
        health = startingHealth;
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
        health -= amount;
        //Player is dead
        if (health <= 0) {
            sr.color = Color.red;
            instance.PlayerControl.Velocity = Vector3.zero;
            instance.PlayerControl.CanMove = false;
            damageable = false;
        }
        //otherwise give them I-Frames
        else {
            StartCoroutine(WaitForIFrames());
        }
        if (SceneManager.GetActiveScene().name == "UI Testing") {
            UIManager.Instance.UpdatePlayerHealth(health / startingHealth);
        }

        ///Wait for a total length of the player's invinicibility length, changing the opacity of the player's sprite a 
        ///number of times set inside the routine
        IEnumerator WaitForIFrames() {
            const int flashCount = 7;
            bool fullOpacity = true;
            for (int i = 0; i < flashCount; i++, fullOpacity = !fullOpacity) {
                Color newColor = sr.color;
                newColor.a = fullOpacity ? 1.0f : 0.5f;
                sr.color = newColor;
                yield return new WaitForSeconds(invincibilityLength / flashCount);
            }
            //ensure that when I-Frames end the player is at full-opacity
            Color fullOpa = sr.color;
            fullOpa.a = 1.0f;
            sr.color = fullOpa;
            damageable = true;
        }
    }

    /// <summary>
    /// Heals the player the given amount without going over their starting health
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount) {
        if (health < startingHealth)
            health += amount;
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
        if (SceneManager.GetActiveScene().name == "UI Testing") {
            UIManager.Instance.EnablePlayerHealthBar(false);
        }
    }

    /// <summary>
    /// Enteres the player into combat, enabling their hitbox and any other combat related UI or restrictions
    /// </summary>
    public void EnterCombat() {
        inCombat = false;
        Hitbox.SetActive(true);
        if (SceneManager.GetActiveScene().name == "UI Testing") {
            UIManager.Instance.EnablePlayerHealthBar(true);
        }
    }

    public void UpdateHitboxHealth() {

    }
}