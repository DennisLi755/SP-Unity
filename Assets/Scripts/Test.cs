using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    void Start() {
        CutsceneManager.FadeObject(gameObject, 0, 10);
    }
}