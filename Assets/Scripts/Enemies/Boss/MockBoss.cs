using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class MockBoss : Boss {
    private Dictionary<GameObject, int> ads = new Dictionary<GameObject, int>();
    [SerializeField]
    private GameObject ad;
    private int totalAds = 2;
    private Coroutine spawnEnemyCoroutine;

    [SerializeField]
    private Vector3[] specialNodes;
    [SerializeField]
    private Vector3[] adNodes;
    private Dictionary<int, bool> adAtNode = new Dictionary<int, bool>();
    [SerializeField]
    GameObject timeoutAd;
    [SerializeField]
    GameObject endingText;

#if UNITY_EDITOR
    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        Handles.color = Color.red;
        foreach (Vector3 node in specialNodes) {
            if (Application.isPlaying) {
                Handles.DrawSolidDisc(node + startPos, Vector3.back, 0.2f);
            }
            else {
                Handles.DrawSolidDisc(node + transform.position, Vector3.back, 0.2f);
            }
        }
        Handles.color = Color.green;
        foreach (Vector3 node in adNodes) {
            if (Application.isPlaying) {
                Handles.DrawSolidDisc(node + startPos, Vector3.back, 0.2f);
            }
            else {
                Handles.DrawSolidDisc(node + transform.position, Vector3.back, 0.2f);
            }
        }
    }
#endif

    private new void Start() {
        base.Start();
        string[] musicLayers = new string[] { "spdemo1_novocals", "spdemo1_full" };
        SoundManager.Instance.SetUpMusicLayers(musicLayers);
        for (int i = 0; i < adNodes.Length; i++) {
            adNodes[i] += transform.position;
            adAtNode.Add(i, false);
        }
        for (int i = 0; i < specialNodes.Length; i++) {
            specialNodes[i] += transform.position;
        }
        //PlayerInfo.Instance.EnableTutorialText("Hold Left Shift to focus", "Hold Right Trgger to focus");
    }

    private new void Update() {
        base.Update();
        //check if Guide should spawn any ads
        if (spawnEnemyCoroutine == null && ads.Count < totalAds) {
            spawnEnemyCoroutine = StartCoroutine(SpawnEnemy());
        }

        //check if any of the ads have been killed; if they have,
        //remove them from the ads list and remove the movementNode they were at from the blacklist
        KeyValuePair<GameObject, int>[] deadAds = ads.Where(ad => !ad.Key.activeInHierarchy).ToArray();
        foreach (KeyValuePair<GameObject, int> kvp in deadAds) {
            adAtNode[kvp.Value] = false;
            ads.Remove(kvp.Key);
        }
    }

    /// <summary>
    /// Spawns a new Ad
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnEnemy() {
        yield return new WaitForSeconds(UnityEngine.Random.Range(2.0f, 3.0f));
        GameObject newAd = Instantiate(ad, transform.position, Quaternion.identity);

        //random node
        int targetIndex = UnityEngine.Random.Range(0, adNodes.Length);
        //ensure that the node doesn't already have an ad
        while (adAtNode[targetIndex]) {
            targetIndex++;
            //keep the index in bounds
            if (targetIndex >= adNodes.Length) {
                targetIndex = 0;
            }
        }
        //track that there is an ad at that node
        adAtNode[targetIndex] = true;
        //tell the ad where to move to
        newAd.GetComponent<RailEnemy>().AddNode(adNodes[targetIndex]);

        ads.Add(newAd, targetIndex);
        //if Guide can spawn more ads, call the routine again
        if (ads.Count < totalAds) {
            StartCoroutine(SpawnEnemy());
        }
        else {
            spawnEnemyCoroutine = null;
        }
    }

    /// <inheritdoc/>
    /// <summary>
    /// Guide changes to phases 1 and 2 at <= 20 health and <= 15 health respectively
    /// </summary>
    public override bool ChangePhase() {
        switch (currentPhase) {
            case 0:
                if (currentHealth <= 15) {
                    currentPhase++;
                    totalAds = 0;
                    StartCoroutine(MoveToTargetNode(specialNodes[1], () => {
                        UIManager.Instance.EnableTimeoutText(true);
                        StartCoroutine(PhaseTimeOut(25));
                        timeoutAd =Instantiate(timeoutAd, transform.position, Quaternion.identity);
                        GetComponent<SpriteRenderer>().enabled = false;
                    }));
                    return true;
                }
                break;
            case 1:  break; //do nothing because its a timeout
        }
        return false;
    }

    IEnumerator PhaseTimeOut(float timeLeft) {
        UIManager.Instance.UpdateTimeoutText(timeLeft);
        while (timeLeft >= 0) {
            timeLeft -= Time.deltaTime;
            UIManager.Instance.UpdateTimeoutText(timeLeft);
            yield return null;
        }
        currentPhase++;
        GetComponent<SpriteRenderer>().enabled = true;
        UIManager.Instance.EnableTimeoutText(false);
        Destroy(timeoutAd);
        totalAds = 4;
    }

    ///
    public override void MoveToNewNode() {
        base.MoveToNewNode();
        StartCoroutine(CreateAfterImages());
    }

    void OnDisable() {
        endingText.SetActive(true);
    }
}