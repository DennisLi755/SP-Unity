using System;
using System.Collections.Generic;

namespace Assets {
    [Serializable]
    public class PlayerSaveData {
        public string scene;
        public string saveLocation;
        public string playerName = "Somni";
        public int[] unlockedSkills = new int[0];
        public int[] equippedSkills = new int[] { -1, -1 };
        public bool watchedOpening = false;
    }
}