using System;
using System.Collections.Generic;

namespace Assets {
    [Serializable]
    public class PlayerSaveData {
        public string scene;
        public string saveLocation;
        public string playerName = "Somni";
        public List<int> unlockedSkills = new List<int>();
        public int[] equippedSkills = new int[] { -1, -1 };
    }
}