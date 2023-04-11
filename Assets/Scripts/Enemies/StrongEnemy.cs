using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongEnemy : MonoBehaviour {

    [SerializeField]
    private GameObject[] walls;
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
    }

    public void Activate() {
        ai.enabled = true;
    }
}