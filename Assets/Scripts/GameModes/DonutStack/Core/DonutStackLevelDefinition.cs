using UnityEngine;

namespace GameModes.DonutStack.Core
{
    [CreateAssetMenu(menuName = "GameModes/DonutStack/LevelDefinition", fileName =  "DonutStackLevelDefinition")]
    public class DonutStackLevelDefinition : ScriptableObject
    {
        [Header("Grid Settings")]
        [Tooltip("Radius of the hexagonal grid")]
        public int gridRadius = 3;
        
        [Header("Game Settings")]
        [Tooltip("Number of stacks per turn")]
        public int stacksPerTurn = 3;
        [Tooltip("Number of same-colored pieces needed to destroy")]
        public int piecesToDestroy = 10;
        
        [Header("Timing Settings")]
        [Tooltip("Delay when processing a match")]
        public float matchProcessDelay = 0.2f;
        [Tooltip("Delay between each piece removed")]
        public float pieceRemoveDelay = 0.05f;
        [Tooltip("Delay after destroying pieces")]
        public float postDestroyDelay = 0.2f;
        [Tooltip("Delay before generating a new turn")]
        public float newTurnDelay = 0.5f;
    }
}
