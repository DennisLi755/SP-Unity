using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHolder : MonoBehaviour {
    private static BulletHolder instance;
    public static BulletHolder Instance => instance;

    private int bulletCount;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public static bool HasChildren() {
        return instance.transform.childCount > 0;
    }

    public void AddBullet() {
        if (bulletCount == 0) {
            PlayerInfo.Instance.EnterCombat();
        }
        bulletCount++;
    }

    public void RemoveBullet() {
        bulletCount--;
        if (bulletCount == 0) {
            PlayerInfo.Instance.ExitCombat();
        }
    }
}