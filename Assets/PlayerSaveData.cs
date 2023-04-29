using System;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField]
        private ProgressionFlags[] progressionFlags;
        [SerializeField]
        private ProgressionFlags[] tutorialFlags;

        /// <summary>
        /// Populates the save data's internal list of flags with structs representing the KVP of the given dictionary
        /// </summary>
        /// <param name="flags"></param>
        public void FillProgressionFlags(Dictionary<string, bool> flags) {
            progressionFlags = new ProgressionFlags[flags.Count];
            int i = 0;
            foreach (KeyValuePair<string, bool> kvp in flags) {
                ProgressionFlags flag = new ProgressionFlags {
                    flagName = kvp.Key,
                    flagActivated = kvp.Value
                };
                progressionFlags[i] = flag;
                i++;
            }
        }

        /// <summary>
        /// Creates a dictionary representation of the save data's internal list of flags
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetProgressionFlags() {
            Dictionary<string, bool> flags = new Dictionary<string, bool>();
            if (progressionFlags == null || progressionFlags.Length == 0) {
                return flags;
            }
            foreach (ProgressionFlags flag in progressionFlags) {
                flags.Add(flag.flagName, flag.flagActivated);
            }
            return flags;
        }

        /// <summary>
        /// Populates the save data's internal list of flags with structs representing the KVP of the given dictionary
        /// </summary>
        /// <param name="flags"></param>
        public void FillTutorialFlags(Dictionary<string, bool> flags) {
            tutorialFlags = new ProgressionFlags[flags.Count];
            int i = 0;
            foreach (KeyValuePair<string, bool> kvp in flags) {
                ProgressionFlags flag = new ProgressionFlags {
                    flagName = kvp.Key,
                    flagActivated = kvp.Value
                };
                tutorialFlags[i] = flag;
                i++;
            }
        }

        /// <summary>
        /// Creates a dictionary representation of the save data's internal list of flags
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetTutorialFlags() {
            Dictionary<string, bool> flags = new Dictionary<string, bool>();
            if (tutorialFlags == null || tutorialFlags.Length == 0) {
                return flags;
            }
            foreach (ProgressionFlags flag in tutorialFlags) {
                flags.Add(flag.flagName, flag.flagActivated);
            }
            return flags;
        }
    }
}