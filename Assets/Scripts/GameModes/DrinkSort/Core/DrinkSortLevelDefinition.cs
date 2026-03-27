using UnityEngine;

namespace GameModes.DrinkSort.Core
{
    [CreateAssetMenu(menuName = "GameModes/DrinkSort/LevelDefinition", fileName = "DrinkSortLevelDefinition")]
    public class DrinkSortLevelDefinition : LevelDefinition
    {
        [System.Serializable]
        private struct ItemTypeWeight
        {
            public SortableItemType itemType;
            [Min(0)] public int weight;
        }

        [Header("Grid Settings")]
        [Tooltip("Width of the tray grid")]
        public int gridWidth = 4;
        [Tooltip("Height of the tray grid")]
        public int gridHeight = 4;
        
        [Header("Item Settings")]
        [Tooltip("Optional per-type spawn weights. If empty, all available types use weight 1")]
        [SerializeField] private ItemTypeWeight[] spawnWeights;
        [Tooltip("Initial tray population percentage. 100 = full trays, 0 = empty trays")]
        [Range(0, 100)]
        public int trayPopulationPercent = 66;
        
        [Header("Tray Reserve Settings")]
        [Tooltip("Initial size of each tray's reserve")]
        public int initialTrayReserveSize = 20;
        
        [Header("Game Settings")]
        [Tooltip("Time limit in seconds")]
        public float timeLimit = 120f;
        
        [Header("Timing Settings")]
        [Tooltip("Delay before initially populating trays")]
        public float initialPopulateDelay = 0.1f;
        [Tooltip("Delay between each item when populating")]
        public float itemPopulateDelay = 0.05f;
        [Tooltip("Delay when processing a match")]
        public float matchProcessDelay = 0.2f;
        [Tooltip("Delay after populating a tray")]
        public float postPopulateDelay = 0.1f;

        public int GetSpawnWeight(SortableItemType itemType)
        {
            if (itemType == SortableItemType.None)
            {
                return 0;
            }

            if (spawnWeights == null || spawnWeights.Length == 0)
            {
                return 1;
            }

            for (int i = 0; i < spawnWeights.Length; i++)
            {
                if (spawnWeights[i].itemType == itemType)
                {
                    return Mathf.Max(0, spawnWeights[i].weight);
                }
            }

            return 1;
        }
    }
}
