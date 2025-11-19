using GameModes.BubbleMerge.Core;
using UnityEngine;

namespace GameModes.BubbleMerge.Gameplay
{
    public class BubbleSpawner : MonoBehaviour
    {
        [SerializeField] private Bubble[] bubblePrefabs;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform bubbleRoot;

        public int CurrentTier { get; private set; }
        public int NextTier { get; private set; }
        public int MaxTier => bubblePrefabs.Length - 1;

        public void Init()
        {
            CurrentTier = GetRandomWeightedTier();
            NextTier = GetRandomWeightedTier();
        }

        public void DropBubble()
        {
            SpawnBubble(CurrentTier, spawnPoint.position);

            CurrentTier = NextTier;
            NextTier = GetRandomWeightedTier();

            BubbleGameManager.Instance.UpdateHUD(CurrentTier, NextTier);
        }

        public Bubble SpawnBubble(int tier, Vector3 position)
        {
            return Instantiate(bubblePrefabs[tier], position, Quaternion.identity, bubbleRoot);
        }

        private int GetRandomWeightedTier()
        {
            int totalWeight = 0;

            foreach (var bubble in bubblePrefabs)
                totalWeight += bubble.SpawnWeight;

            if (totalWeight <= 0)
                return 0;

            int randomValue = Random.Range(0, totalWeight);
            int cumulative = 0;

            for (int i = 0; i < bubblePrefabs.Length; i++)
            {
                cumulative += bubblePrefabs[i].SpawnWeight;
                if (randomValue < cumulative)
                    return i;
            }

            return 0;
        }
    }
}
