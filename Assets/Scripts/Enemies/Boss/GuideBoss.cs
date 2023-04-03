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
    private new void Start() {
        base.Start();
        //string[] musicLayers = new string[] {"spdemo1_novocals", "spdemo1_full"}; 
        //SoundManager.Instance.SetUpMusicLayers(musicLayers);
    }

    private new void Update() {
        base.Update();
        //check if Guide should spawn any ads
        if (currentPhase >= 1 && totalAds > 0) {
            if (spawnEnemyCoroutine == null && ads.Count < totalAds) {
                spawnEnemyCoroutine = StartCoroutine(SpawnEnemy());
            }
        }

        //check if any of the ads have been killed; if they have,
        //remove them from the ads list and remove the movementNode they were at from the blacklist
        for (int i = 0; i < ads.Count; i++) {
            if (!ads[i].activeInHierarchy) {
                blacklistNodeIndices.Remove(adNodeIndicies[ads[i]]);
                adNodeIndicies.Remove(ads[i]);
                ads.Remove(ads[i]);
                i--;
            }
        }
    }

    /// <summary>
    /// Spawns a new Ad
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnEnemy() {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        GameObject newAd = Instantiate(ad, transform.position, Quaternion.identity);
        int targetIndex = GetRandomNodeIndex();
        blacklistNodeIndices.Add(targetIndex);
        //used to keep track of what movementNode index an ad was at when it comes time to remove it when it dies
        adNodeIndicies.Add(newAd, targetIndex);
        //tell the ad where to move to
        newAd.GetComponent<RailEnemy>().AddNode(movementNodes[targetIndex]);
        ads.Add(newAd);
        //if Guide can spawn more ads, call the routine again
        if (ads.Count < totalAds) {
            StartCoroutine(SpawnEnemy());
        } else {
            spawnEnemyCoroutine = null;
        }
    }

    /// <inheritdoc/>
    /// <summary>
    /// Guide changes to phases 1 and 2 at <= 20 health and <= 15 health respectively
    /// </summary>
    public override bool ChangePhase() {
        if (currentHealth <= 20 && currentPhase < 1) {
            currentPhase = 1;
            totalAds = 2;
            return true;
        } else if (currentHealth <= 15 && currentPhase < 2) {
            SoundManager.Instance.ChangeMusicLayer(3f);
            currentPhase = 2;
            totalAds = 4;
            return true;
        }
        return false;
    }

    public override void Death()
    {
        for (int i = 0; i < ads.Count; i++) {
            Destroy(ads[i].gameObject);
            //ads.Remove(ads[i]);
        }
        ads.Clear();
        SoundManager.Instance.FadeOutCurrentLayer(3.0f);
        DialogueManager.Instance.StartDialogue("After_Guide");
        base.Death();
    }

    ///
    public override void MoveToNewNode() {
        base.MoveToNewNode();
        StartCoroutine(CreateAfterImages());
    }
}