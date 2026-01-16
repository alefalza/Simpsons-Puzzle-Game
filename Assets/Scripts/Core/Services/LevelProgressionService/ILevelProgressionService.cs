using UnityEngine;

namespace Core.Services.LevelProgressionService
{
    /// <summary>
    /// Interface for level progression service
    /// </summary>
    public interface ILevelProgressionService : IService
    {
        /// <summary>
        /// Get the next playable level number for a game mode
        /// </summary>
        int GetNextPlayableLevel(string gameModeName);

        /// <summary>
        /// Get the LevelDefinition for the next playable level of a game mode
        /// </summary>
        LevelDefinition GetNextPlayableLevelDefinition(string gameModeName);

        /// <summary>
        /// Mark a level as completed
        /// </summary>
        void CompleteLevel(string gameModeName, int levelNumber);

        /// <summary>
        /// Get the highest completed level for a game mode
        /// </summary>
        int GetLastCompletedLevel(string gameModeName);
    }
}
