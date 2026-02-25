using UnityEngine;

namespace GameModes.BubbleMerge.Core
{
    [CreateAssetMenu(menuName = "GameModes/BubbleMerge/LevelDefinition",  fileName = "BubbleMergeLevelDefinition")]
    public class BubbleMergeLevelDefinition : LevelDefinition
    {
        [Header("Score Settings")]
        [Tooltip("Points awarded per tier when merging")]
        public int scorePerTier = 10;

        [Header("Win Condition")]
        [Tooltip("Target score required to complete the level. Set to 0 to disable score-based win condition.")]
        public int targetScore = 0;

        [Header("Spawn Settings")]
        [Tooltip("Optional per-tier spawn weights overriding bubble prefab weights. Index = bubble tier (0 = first prefab).")]
        [SerializeField] private int[] spawnWeightsPerTier;

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
    }
}
