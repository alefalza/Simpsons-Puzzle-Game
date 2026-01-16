using System.Collections.Generic;
using System.Linq;
using GameModes.BubbleMerge.Core;
using GameModes.DrinkSort.Core;
using GameModes.DonutStack.Core;
using UnityEngine;

namespace Core.Services.LevelProgressionService
{
    /// <summary>
    /// Service for managing player level progression and loading appropriate levels
    /// </summary>
    public class LevelProgressionService : ILevelProgressionService
    {
        private LevelProgressionData progressionData;
        private Dictionary<string, List<LevelDefinition>> levelDefinitionsCache;

        public LevelProgressionService() { }
        
        public void Initialize()
        {
            Debug.Log("[LevelProgressionService] Initializing...");
            
            progressionData = LevelProgressionData.Load();
            levelDefinitionsCache = new Dictionary<string, List<LevelDefinition>>();
            
            // Pre-load all level definitions by game mode
            LoadLevelDefinitionsCache();
        }

        private void LoadLevelDefinitionsCache()
        {
            // Load all level definitions from Resources folder
            // Note: Level definitions should be in Resources/LevelDefinitions/ folder
            
            var bubbleMergeLevels = Resources.LoadAll<BubbleMergeLevelDefinition>("LevelDefinitions/BubbleMerge")
                .Cast<LevelDefinition>()
                .OrderBy(ld => ld.name)
                .ToList();
            levelDefinitionsCache["BubbleMerge"] = bubbleMergeLevels;

            var drinkSortLevels = Resources.LoadAll<DrinkSortLevelDefinition>("LevelDefinitions/DrinkSort")
                .Cast<LevelDefinition>()
                .OrderBy(ld => ld.name)
                .ToList();
            levelDefinitionsCache["DrinkSort"] = drinkSortLevels;

            var donutStackLevels = Resources.LoadAll<DonutStackLevelDefinition>("LevelDefinitions/DonutStack")
                .Cast<LevelDefinition>()
                .OrderBy(ld => ld.name)
                .ToList();
            levelDefinitionsCache["DonutStack"] = donutStackLevels;

            Debug.Log($"[LevelProgressionService] Loaded level definitions: " +
                     $"BubbleMerge={bubbleMergeLevels.Count}, " +
                     $"DrinkSort={drinkSortLevels.Count}, " +
                     $"DonutStack={donutStackLevels.Count}");
        }

        public int GetNextPlayableLevel(string gameModeName)
        {
            if (progressionData == null)
            {
                Debug.LogError("[LevelProgressionService] Progression data is null!");
                return 1;
            }

            return progressionData.GetNextPlayableLevel(gameModeName);
        }

        public LevelDefinition GetNextPlayableLevelDefinition(string gameModeName)
        {
            int nextPlayableLevel = GetNextPlayableLevel(gameModeName);
            return GetLevelDefinition(gameModeName, nextPlayableLevel);
        }

        /// <summary>
        /// Get a specific level definition for a game mode
        /// </summary>
        private LevelDefinition GetLevelDefinition(string gameModeName, int levelNumber)
        {
            if (!levelDefinitionsCache.TryGetValue(gameModeName, out var levels) || levels == null || levels.Count == 0)
            {
                Debug.LogWarning($"[LevelProgressionService] No level definitions found for {gameModeName}");
                return null;
            }

            // Level numbers are 1-indexed, list is 0-indexed
            int index = levelNumber - 1;
            
            if (index < 0 || index >= levels.Count)
            {
                // If level doesn't exist, return the last available level
                Debug.LogWarning($"[LevelProgressionService] Level {levelNumber} not found for {gameModeName}, " +
                               $"returning last available level ({levels.Count})");
                return levels[levels.Count - 1];
            }

            return levels[index];
        }

        public void CompleteLevel(string gameModeName, int levelNumber)
        {
            if (progressionData == null)
            {
                Debug.LogError("[LevelProgressionService] Progression data is null!");
                return;
            }

            progressionData.CompleteLevel(gameModeName, levelNumber);
            progressionData.Save();
            
            Debug.Log($"[LevelProgressionService] Completed level {levelNumber} for {gameModeName}");
        }

        public int GetLastCompletedLevel(string gameModeName)
        {
            if (progressionData == null)
            {
                return 0;
            }

            return progressionData.GetLastCompletedLevel(gameModeName);
        }

        public void Shutdown()
        {
            Debug.Log("[LevelProgressionService] Shutting down...");
            levelDefinitionsCache?.Clear();
        }
    }
}
