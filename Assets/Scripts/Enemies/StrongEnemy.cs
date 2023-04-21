using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongEnemy : MonoBehaviour {

    [SerializeField]
    private GameObject[] walls;
    [SerializeField]
    private StaticEnemy[] ads;
    public StaticEnemy[] Ads { get => ads; }
    StaticEnemy ai;

    private void Awake() {
        //disable the enemy AI before it starts
        StaticEnemy staticAI;
        if (TryGetComponent<StaticEnemy>(out staticAI)) {
            ai = staticAI;
        }
        else {
            RailEnemy railAI;
            if (TryGetComponent<RailEnemy>(out railAI)) {
                ai = railAI;
            }
        }
        ai.enabled = false;
        foreach (GameObject wall in walls) {
            wall.SetActive(false);
        }
    }

    public void Activate() {
        ai.enabled = true;
        foreach (GameObject wall in walls) {
            wall.SetActive(true);
        }
    }

    private void OnDisable() {
        foreach (GameObject wall in walls) {
            wall.SetActive(false);
        }
        if (ads.Length != 0) {
            foreach (StaticEnemy ad in ads) {
                if (ad != null) {
                    ad.gameObject.SetActive(false);
                }
            }
        }
        PlayerInfo.Instance?.Heal(99);
    }
}