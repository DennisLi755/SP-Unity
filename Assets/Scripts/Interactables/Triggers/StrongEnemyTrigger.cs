using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongEnemyTrigger : Trigger {
    [SerializeField]
    StrongEnemy strongEnemy;

    protected override void OnTrigger() {
        strongEnemy.Activate();
        base.OnTrigger();
    }
}