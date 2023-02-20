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
    private GameObject ad;
    private int totalAds = 2;
    private Coroutine spawnEnemyCoroutine;
    private Dictionary<GameObject, int> adNodeIndicies = new Dictionary<GameObject, int>();

    private new void Update() {
        base.Update();
        if (currentPhase >= 1 && totalAds > 0) {
            if (spawnEnemyCoroutine == null && ads.Count < totalAds) {
                spawnEnemyCoroutine = StartCoroutine(SpawnEnemy());
            }
        }

        for (int i = 0; i < ads.Count; i++) {
            if (!ads[i].activeInHierarchy) {
                blacklistNodeIndices.Remove(adNodeIndicies[ads[i]]);
                adNodeIndicies.Remove(ads[i]);
                ads.Remove(ads[i]);
            }
        }
    }

    IEnumerator SpawnEnemy() {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        GameObject newAd = Instantiate(ad, transform.position, Quaternion.identity);
        MoveAd(newAd);
        ads.Add(newAd);
        Debug.Log("Created Ad");
        if (ads.Count < totalAds) {
            StartCoroutine(SpawnEnemy());
        } else {
            spawnEnemyCoroutine = null;
        }
    }

    public void MoveAd(GameObject ad) {
        ad.GetComponent<GuideAd>().HasArrived = false;
        int targetIndex = GetRandomNodeIndex();
        ad.GetComponent<StaticEnemy>().IsDamageable = false;
        blacklistNodeIndices.Add(targetIndex);
        StartCoroutine(MoveAdToTargetNode());

        IEnumerator MoveAdToTargetNode() {
            while (ad.transform.position != movementNodes[targetIndex]) {
                ad.transform.position = Vector3.MoveTowards(ad.transform.position, movementNodes[targetIndex], moveSpeed * Time.deltaTime);
                yield return null;
            }
            ad.GetComponent<StaticEnemy>().IsDamageable = true;
            adNodeIndicies[ad] = targetIndex;
            ad.GetComponent<GuideAd>().HasArrived = true;
        }
    }

    public override bool ChangePhase()
    {
        if (currentHealth <= 20 && currentPhase != 1) {
            currentPhase = 1;
            totalAds = 2;
            return true;
        } else if (currentHealth <= 15 && currentPhase != 2) {
            currentPhase = 2;
            totalAds = 4;
            return true;
        }
        return false;
    }
}
