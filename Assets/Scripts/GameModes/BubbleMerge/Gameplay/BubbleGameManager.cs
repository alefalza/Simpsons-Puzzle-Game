using GameModes.BubbleMerge.Core;
using GameModes.BubbleMerge.UI;
using UnityEngine;

namespace GameModes.BubbleMerge.Gameplay
{
    public class BubbleGameManager : MonoBehaviour
    {
        public static BubbleGameManager Instance;

        [SerializeField] private Bubble[] bubblePrefabs;  // Tiers 0..N
        [SerializeField] private BubbleSpawner spawner;
        [SerializeField] private BubbleHUDController bubbleHUDController;

        private BubbleMergeSystem mergeSystem;
        private int score = 0;

        public BubbleSpawner BubbleSpawner => spawner;
        
        private void Awake()
        {
            Instance = this;
            mergeSystem = new BubbleMergeSystem();
        }

        private void Start()
        {
            spawner.Init();
            //UpdateHUD(spawner.GetCurrentTier(), spawner.GetNextTier());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                spawner.DropBubble();
            }
        }

        public Bubble SpawnBubble(int tier, Vector3 position)
        {
            return Instantiate(bubblePrefabs[tier], position, Quaternion.identity);
        }

        public Bubble SpawnMergedBubble(int tier, Vector3 position)
        {
            AddScore(tier * 10);
            return Instantiate(bubblePrefabs[tier], position, Quaternion.identity);
        }

        public void TryMerge(Bubble a, Bubble b)
        {
            mergeSystem.Merge(a, b);
        }

        private void AddScore(int amount)
        {
            score += amount;
            bubbleHUDController.UpdateScore(score);
        }

        public void UpdateHUD(int current, int next)
        {
            bubbleHUDController.UpdateCurrentBubble(current);
            bubbleHUDController.UpdateNextBubble(next);
        }
    }
}
