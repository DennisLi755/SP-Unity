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
                instance = new PlayerInfo();
            }
            return instance;
        }
    }

    #region Stats
    private float health;
    public float Health => health;
    private const float startingHealth = 5;
    private const float invincibilityLength = 1;
    private bool damageable = true;

    private bool canInteract;
    public bool CanInteract => canInteract;
    private InteractableObject interactable;
    public InteractableObject Interactable { get => interactable; set { interactable = value; } }
    #endregion

    private SpriteRenderer sr;
    private PlayerControl playerControl;
    public PlayerControl PlayerControl { get => playerControl; }
    public bool Damagable {get => damageable; set {damageable = value;} }

    [SerializeField]
    private bool attackUnlocked = true;
    public bool AttackUnlocked { get => attackUnlocked; set => attackUnlocked = value; }

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
        playerControl = GetComponent<PlayerControl>();
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
            UIManager.Instance.UpdateHealth(health / startingHealth);
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
}