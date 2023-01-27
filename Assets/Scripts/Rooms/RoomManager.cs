using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    private RoomManager instance;
    public RoomManager Instance {
        get {
            if (instance == null) {
                instance = new RoomManager();
            }
            return instance;
        }
    }

    [SerializeField]
    List<Room> rooms = new List<Room>();

    void Start() {

    }

    void Update() {

    }
}