using GameModes.BubbleMerge.Core;
using GameModes.BubbleMerge.UI;
using GameModes.Core;
using UnityEngine;

namespace GameModes.BubbleMerge.Gameplay
{
    public class BubbleGameManager : BaseGameManager<BubbleGameManager>
    {
        [Header("Level Configuration")]
        [SerializeField] private BubbleMergeLevelDefinition levelData;
        
        [SerializeField] private BubbleSpawner spawner;

        private int score = 0;
        
        private BubbleHUDController BubbleHUDController => hudController as BubbleHUDController;
        
        public int MaxTier => spawner.MaxTier;
        
        private int ScorePerTier => levelData != null ? levelData.scorePerTier : GameConstants.BubbleMerge.ScorePerTier;

        protected override void Start()
        {
            base.Start();
            score = 0;
            spawner.Init();
            InitHUDController();
        }

        private void InitHUDController()
        {
            if (BubbleHUDController != null)
            {
                BubbleHUDController.UpdateScore(0);
                UpdateHUD(spawner.CurrentTier, spawner.NextTier);
            }
        }

        public void UpdateHUD(int current, int next)
        {
            if (BubbleHUDController != null)
            {
                BubbleHUDController.UpdateCurrentBubbleIcon(current);
                BubbleHUDController.UpdateNextBubbleIcon(next);
            }
        }

        public Bubble SpawnMergedBubble(int tier, Vector3 position)
        {
            AddScore(tier * ScorePerTier);
            
            return spawner.SpawnBubble(tier, position);
        }

        public Bubble GetBubblePrefabByTier(int tier)
        {
            return spawner.BubblePrefabs[tier];
        }
        
        private void AddScore(int amount)
        {
            score += amount;
            
            if (BubbleHUDController != null)
            {
                BubbleHUDController.UpdateScore(score);
            }
        }

        protected override void OnPaused()
        {
            base.OnPaused();
            spawner.enabled = false;
        }

        protected override void OnResumed()
        {
            base.OnResumed();
            spawner.enabled = true;
        }

        public void OnGameOver()
        {
            IsInputBlocked = true;
            
            if (BubbleHUDController != null)
            {
                BubbleHUDController.ShowGameOverOverlay(score);
            }
        }
    }
}
