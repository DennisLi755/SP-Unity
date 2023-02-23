using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHolder : MonoBehaviour {
    private static BulletHolder instance;
    public static BulletHolder Instance => instance;

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
}