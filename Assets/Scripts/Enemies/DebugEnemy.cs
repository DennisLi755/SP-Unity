using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Events;

public class DebugEnemy : MonoBehaviour {

    [SerializeField]
    GameObject bullet;
    [SerializeField]
    UnityEvent unityEvents;

    void Start() {

    }

    void Update() {
        
    }

    [ContextMenu("Spawn Bullet")]
    private void SpawnBullet() {
        Instantiate(bullet, transform);
    }

    public void Wait(float time) {

    }

    public void SpawnBulletPattern(GameObject pattern) {

    }
}
