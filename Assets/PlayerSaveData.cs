using System;
using System.Collections.Generic;

namespace Assets {
    [Serializable]
    public struct ProgressionFlags {
        public string flagName;
        public bool flagActivated;
    }

    [Serializable]
    public class PlayerSaveData {
        public string scene;
        public string saveLocation;
        public string playerName = "Somni";
        public int[] unlockedSkills = new int[0];
        public int[] equippedSkills = new int[] { -1, -1 };
        public ProgressionFlags[] progressionFlags;

        public void FillProgressionFlags(Dictionary<string, bool> flags) {
            progressionFlags = new ProgressionFlags[flags.Count];
            int i = 0;
            foreach (KeyValuePair<string, bool> kvp in flags) {
                ProgressionFlags flag = new ProgressionFlags();
                flag.flagName = kvp.Key;
                flag.flagActivated = kvp.Value;
                progressionFlags[i] = flag;
                i++;
            }
        }

        public Dictionary<string, bool> GetProgressionFlags() {
            Dictionary<string, bool> flags = new Dictionary<string, bool>();
            foreach (ProgressionFlags flag in progressionFlags) {
                flags.Add(flag.flagName, flag.flagActivated);
            }
            return flags;
        }
    }
}