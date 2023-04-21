using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongEnemyTrigger : Trigger {
    [SerializeField]
    StrongEnemy strongEnemy;
    protected override void OnTrigger() {
        if (strongEnemy.Ads.Length != 0) {
            foreach (StaticEnemy ad in strongEnemy.Ads) {
                ad.Activate();
            }
        }
        PlayerInfo.Instance.Heal(99);
        strongEnemy.Activate();
        base.OnTrigger();
    }
}