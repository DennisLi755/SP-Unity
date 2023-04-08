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

    /// <summary>
    /// Whether there are any bullets in the scene currently
    /// </summary>
    /// <returns></returns>
    public static bool HasChildren() {
        return instance.transform.childCount > 0;
    }

    /// <summary>
    /// Adds a bullet to the bullet count, if the count was 0 before enters the player into combat
    /// </summary>
    public void AddBullet() {
        if (bulletCount == 0) {
            PlayerInfo.Instance.EnterCombat();
        }
        bulletCount++;
    }

    /// <summary>
    /// Reduces the bullet count, if 0 afterwards removes the player from combat
    /// </summary>
    public void RemoveBullet() {
        bulletCount--;
        if (bulletCount == 0) {
            PlayerInfo.Instance?.ExitCombat();
        }
    }

    /// <summary>
    /// Removes all bullets in the scene
    /// </summary>
    [ContextMenu("Cull Bullets")]
    public void ClearBullets() {
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        foreach (Bullet b in bullets) {
            Destroy(b.gameObject);
        }
        BulletPattern[] bulletPatterns = FindObjectsOfType<BulletPattern>();
        foreach (BulletPattern b in bulletPatterns) {
            Destroy(b.gameObject);
        }
    }
}