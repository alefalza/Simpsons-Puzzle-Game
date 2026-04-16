using UnityEngine;

namespace GameModes.DonutStack.Core
{
    [CreateAssetMenu(menuName = "GameModes/DonutStack/LevelDefinition", fileName = "DonutStackLevelDefinition")]
    public class DonutStackLevelDefinition : LevelDefinition
    {
        [System.Serializable]
        private struct ItemTypeWeight
        {
            public DonutColor color;
            [Min(0)] public int weight;
        }
        
        [Header("Grid Settings")]
        [Tooltip("Radius of the hexagonal grid")]
        public int gridRadius = 1;
        
        [Header("Game Settings")]
        [Tooltip("Number of stacks per turn")]
        public int stacksPerTurn = 3;
        [Tooltip("Number of same-colored pieces needed to destroy")]
        public int piecesToDestroy = 10;
        
        [Header("Item Settings")]
        [Tooltip("Optional per-type spawn weights. If empty, all available types use weight 1")]
        [SerializeField] private ItemTypeWeight[] spawnWeights;
        
        [Header("Timing Settings")]
        [Tooltip("Delay when processing a match")]
        public float matchProcessDelay = 0.2f;
        [Tooltip("Delay between each piece removed")]
        public float pieceRemoveDelay = 0.05f;
        [Tooltip("Delay after destroying pieces")]
        public float postDestroyDelay = 0.2f;
        [Tooltip("Delay before generating a new turn")]
        public float newTurnDelay = 0.5f;
        
        public int GetSpawnWeight(DonutColor donutColor)
        {
            if (donutColor == DonutColor.None)
            {
                return 0;
            }

            if (spawnWeights == null || spawnWeights.Length == 0)
            {
                return 1;
            }

            for (int i = 0; i < spawnWeights.Length; i++)
            {
                if (spawnWeights[i].color == donutColor)
                {
                    return Mathf.Max(0, spawnWeights[i].weight);
                }
            }

            return 1;
        }
    }
}
