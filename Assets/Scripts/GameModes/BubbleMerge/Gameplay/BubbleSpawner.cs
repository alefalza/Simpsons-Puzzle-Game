using UnityEngine;

namespace GameModes.BubbleMerge.Gameplay
{
    public class BubbleSpawner : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform bubbleRoot;

        private int currentTier;
        private int nextTier;

        public void Init()
        {
            currentTier = RandomTier();
            nextTier = RandomTier();
        }

        public void DropBubble()
        {
            BubbleGameManager.Instance.SpawnBubble(currentTier, spawnPoint.position, bubbleRoot);

            currentTier = nextTier;
            nextTier = RandomTier();

            BubbleGameManager.Instance.UpdateHUD(currentTier, nextTier);
        }

        private int RandomTier()
        {
            // Only small tiers for spawning
            return Random.Range(0, 3);
        }

        public int GetCurrentTier() => currentTier;
        public int GetNextTier() => nextTier;
    }
}
