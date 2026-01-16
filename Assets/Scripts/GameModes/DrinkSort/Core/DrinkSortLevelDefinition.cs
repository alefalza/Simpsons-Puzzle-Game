using UnityEngine;

namespace GameModes.DrinkSort.Core
{
    [CreateAssetMenu(menuName = "GameModes/DrinkSort/LevelDefinition", fileName = "DrinkSortLevelDefinition")]
    public class DrinkSortLevelDefinition : LevelDefinition
    {
        [Header("Score Settings")]
        [Tooltip("Points awarded per match")]
        public int scorePerMatch = 10;
        
        [Header("Grid Settings")]
        [Tooltip("Width of the tray grid")]
        public int gridWidth = 4;
        [Tooltip("Height of the tray grid")]
        public int gridHeight = 4;
        
        [Header("Item Settings")]
        [Tooltip("Initial amount of items per tray")]
        public int initialItemsPerTray = 2;
        [Tooltip("Amount of items to add when a tray is cleared")]
        public int itemsToFillOnClear = 3;
        
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
    }
}
