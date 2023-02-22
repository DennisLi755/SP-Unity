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
    private bool damagable = true;

    private bool canInteract;
    public bool CanInteract => canInteract;
    private InteractableObject interactable;
    public InteractableObject Interactable { get => interactable; set { interactable = value; } }
    #endregion

    private SpriteRenderer sr;
    private PlayerControl playerControl;
    public PlayerControl PlayerControl { get => playerControl; }
    public bool Damagable {get => damagable; set {damagable = value;} }

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

    public void Hurt(int amount) {
        if (!damagable) {
            return;
        }
        damagable = false;
        health -= amount;
        if (health <= 0) {
            sr.color = Color.red;
        }
        else {
            StartCoroutine(WaitForIFrames());
        }
        if (SceneManager.GetActiveScene().name == "UI Testing") {
            UIManager.Instance.UpdateHealth(health / startingHealth);
        }

        IEnumerator WaitForIFrames() {
            const int flashCount = 7;
            bool fullOpacity = true;
            for (int i = 0; i < flashCount; i++, fullOpacity = !fullOpacity) {
                Color newColor = sr.color;
                newColor.a = fullOpacity ? 1.0f : 0.5f;
                sr.color = newColor;
                yield return new WaitForSeconds(invincibilityLength / flashCount);
            }
            Color fullOpa = sr.color;
            fullOpa.a = 1.0f;
            sr.color = fullOpa;
            damagable = true;
        }
    }

    public void Heal(int amount) {
        if (health < startingHealth)
            health += amount;
    }

    public void EnterInteractable(InteractableObject sender) {
        canInteract = true;
        interactable = sender;
    }

    public void ExitInteractable(InteractableObject sender) {
        if (interactable == sender) {
            canInteract = false;
            interactable = null;
        }
    }

    public void ChangeInputMap(string map) {
        GetComponent<PlayerInput>().SwitchCurrentActionMap(map);
    }
}