using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

    private static PlayerInfo instance;

    #region Stats
    public float health;
    public float startingHealth;
    public float invincibilityLength;
    public bool damagable;

    public bool canInteract;
    public InteractableObject interactable;
    #endregion

    public static PlayerInfo Instance {
        get {
            if (instance == null) {
                instance = new PlayerInfo();
            }
            return instance;
        }
    }

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
        
    }
}