using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.LevelProgressionService
{
    /// <summary>
    /// Data class for storing player level progression
    /// </summary>
    [System.Serializable]
    public class LevelProgressionData
    {
        public Dictionary<string, int> completedLevels = new Dictionary<string, int>();

        private const string KEY = "LEVEL_PROGRESSION_DATA";

        /// <summary>
        /// Get the next playable level for a game mode
        /// </summary>
        public int GetNextPlayableLevel(string gameModeName)
        {
            if (completedLevels.TryGetValue(gameModeName, out int lastCompleted))
            {
                return lastCompleted + 1; // Next level after last completed
            }
            
            return 1;
        }

        /// <summary>
        /// Mark a level as completed for a game mode
        /// </summary>
        public void CompleteLevel(string gameModeName, int levelNumber)
        {
            if (completedLevels.TryGetValue(gameModeName, out int lastCompleted))
            {
                if (levelNumber > lastCompleted)
                {
                    completedLevels[gameModeName] = levelNumber;
                }
            }
            else
            {
                completedLevels[gameModeName] = levelNumber;
            }
        }

        /// <summary>
        /// Get the highest completed level for a game mode
        /// </summary>
        public int GetLastCompletedLevel(string gameModeName)
        {
            if (completedLevels.TryGetValue(gameModeName, out int lastCompleted))
            {
                return lastCompleted;
            }
            
            return 0; // No levels completed
        }

        public void Save()
        {
            gameModeNames.Clear();
            levelNumbers.Clear();
            
            foreach (var kvp in completedLevels)
            {
                gameModeNames.Add(kvp.Key);
                levelNumbers.Add(kvp.Value);
            }
            
            var serializable = new SerializableDictionary(gameModeNames, levelNumbers);
            string json = JsonUtility.ToJson(serializable);
            PlayerPrefs.SetString(KEY, json);
            PlayerPrefs.Save();
        }

        public static LevelProgressionData Load()
        {
            if (!PlayerPrefs.HasKey(KEY))
                return new LevelProgressionData();

            string json = PlayerPrefs.GetString(KEY);
            var serializable = JsonUtility.FromJson<SerializableDictionary>(json);
            
            var data = new LevelProgressionData();
            
            if (serializable != null && serializable.gameModeNames != null && serializable.levelNumbers != null)
            {
                for (int i = 0; i < serializable.gameModeNames.Count && i < serializable.levelNumbers.Count; i++)
                {
                    data.completedLevels[serializable.gameModeNames[i]] = serializable.levelNumbers[i];
                }
            }
            
            return data;
        }
        
        /// <summary>
        /// Reset progression in-memory (does not persist unless you call Save()).
        /// </summary>
        public void ResetInMemory()
        {
            completedLevels.Clear();
            gameModeNames.Clear();
            levelNumbers.Clear();
        }

        /// <summary>
        /// Reset progression and persist the reset (clears PlayerPrefs entry).
        /// </summary>
        public void ResetAndDeleteSaved()
        {
            ResetInMemory();
            DeleteSaved();
        }

        /// <summary>
        /// Deletes the saved progression data from PlayerPrefs.
        /// </summary>
        public static void DeleteSaved()
        {
            if (PlayerPrefs.HasKey(KEY))
            {
                PlayerPrefs.DeleteKey(KEY);
                PlayerPrefs.Save();
            }
        }

        [SerializeField] private List<string> gameModeNames = new List<string>();
        [SerializeField] private List<int> levelNumbers = new List<int>();

        [System.Serializable]
        private class SerializableDictionary
        {
            public List<string> gameModeNames;
            public List<int> levelNumbers;

            public SerializableDictionary(List<string> names, List<int> levels)
            {
                gameModeNames = names;
                levelNumbers = levels;
            }
        }
    }
}
