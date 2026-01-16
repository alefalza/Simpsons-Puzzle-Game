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
    }
}
