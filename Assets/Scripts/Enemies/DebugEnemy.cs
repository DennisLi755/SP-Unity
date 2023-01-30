using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class DebugEnemy : MonoBehaviour {

    [SerializeField]
    GameObject bullet;

    void Start() {

    }

    void Update() {
        
    }

    [ContextMenu("Spawn Bullet")]
    private void SpawnBullet() {
        Instantiate(bullet, transform);
    }
}
