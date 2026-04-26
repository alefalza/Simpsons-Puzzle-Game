using UnityEngine;

namespace GameModes.BubbleMerge.Core
{
    [CreateAssetMenu(menuName = "GameModes/BubbleMerge/LevelDefinition",  fileName = "BubbleMergeLevelDefinition")]
    public class BubbleMergeLevelDefinition : LevelDefinition
    {
        [System.Serializable]
        private struct ItemTypeWeight
        {
            public int tier;
            [Min(0)] public int weight;
        }
        
        [Header("Spawn Settings")]
        [Tooltip("Optional per-tier spawn weights overriding bubble prefab weights. Index = bubble tier (0 = first prefab).")]
        [SerializeField] private int[] spawnWeightsPerTier;

        [Header("Item Settings")]
        [Tooltip("Optional per-type spawn weights. If empty, all available types use weight 1")]
        [SerializeField] private ItemTypeWeight[] spawnWeights;
        
        /// <summary>
        /// Returns the per-tier spawn weights for this level.
        /// If not configured or empty, returns null so that prefab weights are used instead.
        /// The returned array length will be clamped to the provided tierCount.
        /// </summary>
        /// <param name="tierCount">Number of tiers (bubble prefabs) available in the spawner.</param>
        public int[] GetSpawnWeights(int tierCount)
        {
            if (spawnWeightsPerTier == null || spawnWeightsPerTier.Length == 0 || tierCount <= 0)
            {
                return null;
            }

            if (spawnWeightsPerTier.Length == tierCount)
            {
                return spawnWeightsPerTier;
            }

            var result = new int[tierCount];
            int copyLength = Mathf.Min(tierCount, spawnWeightsPerTier.Length);

            for (int i = 0; i < copyLength; i++)
            {
                result[i] = spawnWeightsPerTier[i];
            }

            return result;
        }
        
        public int GetSpawnWeight(int tier)
        {
            if (tier == -1)
            {
                return 0;
            }

            if (spawnWeights == null || spawnWeights.Length == 0)
            {
                return 1;
            }

            for (int i = 0; i < spawnWeights.Length; i++)
            {
                if (spawnWeights[i].tier == -1)
                {
                    return Mathf.Max(0, spawnWeights[i].weight);
                }
            }

            return 1;
        }
    }
}
