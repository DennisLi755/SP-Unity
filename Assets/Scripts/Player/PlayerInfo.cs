using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

    private static PlayerInfo instance;

    #region Stats
    private float health;
    public float Health => health;
    private const float startingHealth = 5;
    private const float invincibilityLength = 1;
    private bool damagable;

    private bool canInteract;
    public bool CanInteract => canInteract;
    private InteractableObject interactable;
    public InteractableObject Interactable { get => interactable; set { interactable = value; } }
    #endregion

    public static PlayerInfo Instance {
        get {
            if (instance == null) {
                instance = new PlayerInfo();
            }
            return instance;
        }
    }

    private PlayerControl playerControl;
    public PlayerControl PlayerControl { get => playerControl; }

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
        health = startingHealth;
    }

    public void Hurt(int amount) {
        if (!damagable) {
            return;
        }
        damagable = false;
        health -= amount;
        if (health <= 0) {
            //do death stuff
        }
        else {
            StartCoroutine(WaitForIFrames());
        }

        IEnumerator WaitForIFrames() {
            yield return new WaitForSeconds(invincibilityLength);
            damagable = true;
        }
    }
}