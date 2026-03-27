using GameModes.BubbleMerge.Core;
using GameModes.BubbleMerge.UI;
using GameModes.Core;
using UnityEngine;

namespace GameModes.BubbleMerge.Gameplay
{
    public class BubbleGameManager : BaseGameManager<BubbleGameManager>
    {
        [SerializeField] private BubbleSpawner spawner;
        
        protected override string GameModeName => "BubbleMerge";
        
        private int score = 0;
        private bool hasWon = false;
        
        private BubbleHUDController BubbleHUDController => hudController as BubbleHUDController;
        
        public int MaxTier => spawner.MaxTier;
        
        private int ScorePerTier => levelData != null ? ((BubbleMergeLevelDefinition)levelData).scorePerTier : GameConstants.BubbleMerge.ScorePerTier;
        private int TargetScore => levelData != null ? ((BubbleMergeLevelDefinition)levelData).targetScore : 0;

        protected override void Start()
        {
            base.Start();            
            score = 0;
            hasWon = false;
            spawner.Init(levelData as BubbleMergeLevelDefinition);
            InitHUDController();
        }

        private void InitHUDController()
        {
            if (BubbleHUDController != null)
            {
                BubbleHUDController.SetLevelText(currentLevelNumber);
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
            
            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            if (hasWon || IsInputBlocked) return;
            
            if (TargetScore > 0 && score >= TargetScore)
            {
                hasWon = true;
                MarkLevelAsCompleted();
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
