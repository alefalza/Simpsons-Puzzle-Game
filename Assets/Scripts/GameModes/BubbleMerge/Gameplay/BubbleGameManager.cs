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
            Bubble bubble = spawner.SpawnBubble(tier, position);
            CheckWinCondition(bubble);
            return bubble;
        }

        public Bubble GetBubblePrefabByTier(int tier)
        {
            return spawner.BubblePrefabs[tier];
        }
        
        private void CheckWinCondition(Bubble bubble)
        {
            if (hasWon || IsInputBlocked) return;
            
            if (bubble.Tier == spawner.MaxTier)
            {
                OnGameWin();
            }
        }
        
        private void OnGameWin()
        {
            hasWon = true;
            IsInputBlocked = true;
            spawner.enabled = false;
            
            MarkLevelAsCompleted();
            
            if (hudController != null)
            {
                hudController.ShowWinPopup(0);
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
