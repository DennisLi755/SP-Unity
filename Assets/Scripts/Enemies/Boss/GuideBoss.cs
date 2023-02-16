using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class GuideBoss : Boss
{
    private List<GameObject> ads = new List<GameObject>();
    [SerializeField]
    private GameObject enemy;
    private int totalAds = 2;
    private Coroutine spawnEnemyCoroutine;
    private System.Random rng = new System.Random();

    private new void Update() {
        base.Update();
        if (currentPhase >= 1) {
            if (spawnEnemyCoroutine == null && ads.Count < totalAds) {
                spawnEnemyCoroutine = StartCoroutine(SpawnEnemy());
            }
        }
        if (currentPhase == 2) {
            totalAds += 2;
        }
        if (currentHealth == 20 || currentHealth == 15)
            currentPhase++;
    }

    IEnumerator SpawnEnemy() {
        yield return new WaitForSeconds((float)rng.Next(1, 3));
        GameObject ad = Instantiate(enemy, transform.position, Quaternion.identity, transform);
        ads.Add(ad);
        if (ads.Count < totalAds) {
            StartCoroutine(SpawnEnemy());
        } else {
            spawnEnemyCoroutine = null;
        }
    }
}
