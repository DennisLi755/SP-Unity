using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}