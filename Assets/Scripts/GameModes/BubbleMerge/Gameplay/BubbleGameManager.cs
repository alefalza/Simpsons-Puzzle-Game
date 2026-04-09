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
        
        private BubbleHUDController BubbleHUDController => hudController as BubbleHUDController;
        
        public int MaxTier => spawner.MaxTier;
        
        protected override void Start()
        {
            base.Start();
            hasWon = false;
            hasLost = false;
            spawner.Init(levelData as BubbleMergeLevelDefinition);
            InitHUDController();
        }

        private void InitHUDController()
        {
            if (BubbleHUDController != null)
            {
                BubbleHUDController.SetLevelText(currentLevelNumber);
                UpdateHUD(spawner.CurrentTier, spawner.NextTier);
            }
        }

        public void UpdateHUD(int current, int next)
        {
            BubbleHUDController.UpdateCurrentBubbleIcon(current);
            BubbleHUDController.UpdateNextBubbleIcon(next);
        }

        public Bubble SpawnMergedBubble(int tier, Vector3 position)
        {
            Bubble bubble = spawner.SpawnBubble(tier, position);
            CheckWinCondition(bubble);
            return bubble;
        }

        public Bubble GetBubblePrefabByTier(int tier)
        {
            return spawner.BubblePrefabs[tier];
        }
        
        protected override void OnPaused()
        {
            base.OnPaused();
            SetSpawnerEnable(false);
        }

        protected override void OnResumed()
        {
            base.OnResumed();
            SetSpawnerEnable(true);
        }
        
        private void SetSpawnerEnable(bool enable)
        {
            spawner.enabled = enable;
        }
        
        private void CheckWinCondition(Bubble bubble)
        {
            if (hasLost) return;
            
            if (bubble.Tier == spawner.MaxTier)
            {
                OnGameWon();
            }
        }
        
        protected override void OnGameWon(int finalScore = 0)
        {
            IsInputBlocked = true;
            
            SetSpawnerEnable(false);
            MarkLevelAsCompleted();
            
            base.OnGameWon(finalScore);
        }

        public void CheckLoseCondition()
        {
            if (hasWon) return;
            
            OnGameLost();
        }
        
        protected override void OnGameLost(int finalScore = 0)
        {
            IsInputBlocked = true;
            
            base.OnGameLost(finalScore);
        }
    }
}
