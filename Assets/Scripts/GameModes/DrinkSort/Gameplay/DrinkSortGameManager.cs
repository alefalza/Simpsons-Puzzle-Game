using System.Collections;
using GameModes.Core;
using GameModes.DrinkSort.Core;
using GameModes.DrinkSort.UI;
using UnityEngine;

namespace GameModes.DrinkSort.Gameplay
{
    public class DrinkSortGameManager : BaseGameManager<DrinkSortGameManager>
    {
        [Header("Grid Settings")]
        [SerializeField] private TrayGrid trayGrid;
        
        [Header("Item Settings")]
        [SerializeField] private ItemReserve itemReserve;

        protected override string GameModeName => "DrinkSort";

        private float currentTime;
        private int score = 0;
        private bool isGameOver = false;
        
        private DrinkSortHUDController DrinkSortHUDController => hudController as DrinkSortHUDController;
        
        // Properties that get values from levelData or default values
        private int GridWidth => levelData != null ? ((DrinkSortLevelDefinition)levelData).gridWidth : 4;
        private int GridHeight => levelData != null ? ((DrinkSortLevelDefinition)levelData).gridHeight : 4;
        private float TimeLimit => levelData != null ? ((DrinkSortLevelDefinition)levelData).timeLimit : 120f;
        private int ScorePerMatch => levelData != null ? ((DrinkSortLevelDefinition)levelData).scorePerMatch : GameConstants.DrinkSort.ScorePerMatch;
        private float MatchProcessDelay => levelData != null ? ((DrinkSortLevelDefinition)levelData).matchProcessDelay : GameConstants.DrinkSort.MatchProcessDelay;
        
        public float TimeRemaining => Mathf.Max(0, TimeLimit - currentTime);
        public int ItemsRemaining => GetTotalItemCount();
        public int Score => score;
        
        protected override void Start()
        {
            base.Start();
            InitializeGame();
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (IsPaused || isGameOver) return;
            
            UpdateTimer();
        }
        
        private void InitializeGame()
        {
            currentTime = 0f;
            score = 0;
            isGameOver = false;
            
            trayGrid.Initialize(
                GridWidth, 
                GridHeight,
                itemReserve != null ? itemReserve.ItemPrefab : null,
                GetColorForType,
                GetRandomAvailableType
            );
            
            UpdateHUD();
        }
        
        private Color GetColorForType(SortableItemType itemType)
        {
            if (itemReserve != null)
            {
                return itemReserve.GetColorForType(itemType);
            }
            
            return Color.white;
        }
        
        private SortableItemType GetRandomAvailableType()
        {
            if (itemReserve != null)
            {
                return itemReserve.GetRandomAvailableType();
            }
            
            return SortableItemType.None;
        }
        
        public void CheckTrayForMatch(Tray tray)
        {
            if (IsInputBlocked || tray == null) return;
            
            if (tray.HasMatch())
            {
                SortableItemType matchedType = tray.GetMatchType();
                StartCoroutine(ProcessMatch(tray, matchedType));
            }
        }
        
        private IEnumerator ProcessMatch(Tray tray, SortableItemType matchedType)
        {
            IsInputBlocked = true;
            
            // Add score
            AddScore(ScorePerMatch);
            
            // Clear tray
            tray.ClearItems();
            
            yield return new WaitForSeconds(MatchProcessDelay);
            
            IsInputBlocked = false;
            
            // Update HUD
            UpdateHUD();
            
            // Check win condition
            CheckWinCondition();
        }
        
        private void UpdateTimer()
        {
            currentTime += Time.deltaTime;
            
            UpdateHUD();
            
            if (TimeRemaining <= 0 && !isGameOver)
            {
                OnGameOver();
            }
        }
        
        private void UpdateHUD()
        {
            if (DrinkSortHUDController != null)
            {
                DrinkSortHUDController.UpdateTimer(TimeRemaining);
                DrinkSortHUDController.UpdateItemsRemaining(GetTotalItemCount());
            }
        }
        
        private int GetTotalItemCount()
        {
            int total = 0;
            
            foreach (var tray in trayGrid.Trays.Values)
            {
                total += tray.ItemCount;
            }
            
            return total;
        }
        
        private void AddScore(int amount)
        {
            score += amount;
            
            if (hudController != null)
            {
                hudController.UpdateScore(score);
            }
        }
        
        private void CheckWinCondition()
        {
            // Win: no items remain in the trays
            bool hasRemainingItems = false;
            
            foreach (var tray in trayGrid.Trays.Values)
            {
                if (!tray.IsEmpty)
                {
                    hasRemainingItems = true;
                    break;
                }
            }
            
            // If there are no items in the trays, you win
            if (!hasRemainingItems)
            {
                OnGameWin();
            }
        }
        
        private void OnGameWin()
        {
            if (isGameOver) return;
            
            isGameOver = true;
            IsInputBlocked = true;
            
            MarkLevelAsCompleted();
            
            if (hudController != null)
            {
                hudController.ShowGameOverOverlay(score);
            }
        }
        
        private void OnGameOver()
        {
            if (isGameOver) return;
            
            isGameOver = true;
            IsInputBlocked = true;
            
            if (hudController != null)
            {
                hudController.ShowGameOverOverlay(score);
            }
        }
    }
}
