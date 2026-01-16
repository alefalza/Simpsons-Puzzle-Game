using UnityEngine;

namespace GameModes.BubbleMerge.Core
{
    [CreateAssetMenu(menuName = "GameModes/BubbleMerge/LevelDefinition",  fileName = "BubbleMergeLevelDefinition")]
    public class BubbleMergeLevelDefinition : ScriptableObject
    {
        [Header("Score Settings")]
        [Tooltip("Points awarded per tier when merging")]
        public int scorePerTier = 10;
    }
}
