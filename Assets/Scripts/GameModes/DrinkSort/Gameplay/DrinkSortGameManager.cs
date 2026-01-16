using System.Collections;
using System.Collections.Generic;
using GameModes.Core;
using GameModes.DrinkSort.Core;
using GameModes.DrinkSort.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModes.DrinkSort.Gameplay
{
    public class DrinkSortGameManager : BaseGameManager<DrinkSortGameManager>
    {
        [Header("Level Configuration")]
        [SerializeField] private DrinkSortLevelDefinition levelData;
        
        [Header("Grid Settings")]
        [SerializeField] private TrayGrid trayGrid;
        
        [Header("Item Settings")]
        [SerializeField] private ItemReserve itemReserve; // To get sprites and prefab
        [SerializeField] private Transform itemsContainer;
        
        [Header("Tray Reserve Settings")]
        [SerializeField] private TrayReserve.ItemData[] trayReserveItems; // Items available for tray reserves
        
        private float currentTime;
        private int totalItemsUsed = 0;
        private int score = 0;
        private bool isGameOver = false;
        
        private DrinkSortHUDController DrinkSortHUDController => hudController as DrinkSortHUDController;
        
        // Properties that get values from levelData or default values
        private int GridWidth => levelData != null ? levelData.gridWidth : 4;
        private int GridHeight => levelData != null ? levelData.gridHeight : 4;
        private int InitialItemsPerTray => levelData != null ? levelData.initialItemsPerTray : 2;
        private int ItemsToFillOnClear => levelData != null ? levelData.itemsToFillOnClear : 3;
        private int InitialTrayReserveSize => levelData != null ? levelData.initialTrayReserveSize : 20;
        private float TimeLimit => levelData != null ? levelData.timeLimit : 120f;
        private int ScorePerMatch => levelData != null ? levelData.scorePerMatch : GameConstants.DrinkSort.ScorePerMatch;
        private float InitialPopulateDelay => levelData != null ? levelData.initialPopulateDelay : GameConstants.DrinkSort.InitialPopulateDelay;
        private float ItemPopulateDelay => levelData != null ? levelData.itemPopulateDelay : GameConstants.DrinkSort.ItemPopulateDelay;
        private float MatchProcessDelay => levelData != null ? levelData.matchProcessDelay : GameConstants.DrinkSort.MatchProcessDelay;
        private float PostPopulateDelay => levelData != null ? levelData.postPopulateDelay : GameConstants.DrinkSort.PostPopulateDelay;
        
        public float TimeRemaining => Mathf.Max(0, TimeLimit - currentTime);
        public int ItemsRemaining => GetTotalReserveCount();
        public int Score => score;
        
        protected override void Start()
        {
            base.Start();
            InitializeGame();
        }
        
        protected override void Update()
        {
            if (IsPaused || isGameOver) return;
            
            base.Update();
            UpdateTimer();
        }
        
        private void InitializeGame()
        {
            currentTime = 0f;
            score = 0;
            totalItemsUsed = 0;
            isGameOver = false;
            
            // Initialize grid
            trayGrid.Initialize(GridWidth, GridHeight);
            
            // Initialize reserves for each tray
            InitializeTrayReserves();
            
            // Populate trays initially
            StartCoroutine(PopulateInitialTrays());
            
            // Update HUD
            UpdateHUD();
        }
        
        private void InitializeTrayReserves()
        {
            foreach (var tray in trayGrid.Trays.Values)
            {
                tray.InitializeReserve(trayReserveItems, InitialTrayReserveSize);
            }
        }
        
        private IEnumerator PopulateInitialTrays()
        {
            yield return new WaitForSeconds(InitialPopulateDelay);
            
            List<Tray> emptyTrays = trayGrid.GetEmptyTrays();
            
            foreach (var tray in emptyTrays)
            {
                int itemsToAdd = Random.Range(1, InitialItemsPerTray + 1);
                
                tray.PopulateFromReserve(
                    itemsToAdd,
                    itemReserve != null ? itemReserve.ItemPrefab : null,
                    GetSpriteForType,
                    itemsContainer != null ? itemsContainer : transform
                );
                
                yield return new WaitForSeconds(ItemPopulateDelay);
            }
        }
        
        private Sprite GetSpriteForType(SortableItemType itemType)
        {
            if (itemReserve != null)
            {
                return itemReserve.GetSpriteForType(itemType);
            }
            
            return null;
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
            
            // Populate tray from its own reserve
            if (tray.Reserve.HasItems() && itemReserve != null)
            {
                tray.PopulateFromReserve(
                    ItemsToFillOnClear,
                    itemReserve.ItemPrefab,
                    GetSpriteForType,
                    itemsContainer != null ? itemsContainer : transform
                );
                
                totalItemsUsed += ItemsToFillOnClear;
            }
            
            yield return new WaitForSeconds(PostPopulateDelay);
            
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
                DrinkSortHUDController.UpdateItemsRemaining(GetTotalReserveCount());
            }
        }
        
        private int GetTotalReserveCount()
        {
            int total = 0;
            
            foreach (var tray in trayGrid.Trays.Values)
            {
                total += tray.ReserveCount;
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
            // Win: use all available items from all reserves
            // and no items remain in the trays
            int totalReserveCount = GetTotalReserveCount();
            
            if (totalReserveCount == 0)
            {
                bool hasRemainingItems = false;
                
                foreach (var tray in trayGrid.Trays.Values)
                {
                    if (!tray.IsEmpty)
                    {
                        hasRemainingItems = true;
                        break;
                    }
                }
                
                // If all reserves are empty and there are no items in the trays, you win
                if (!hasRemainingItems)
                {
                    OnGameWin();
                }
            }
        }
        
        private void OnGameWin()
        {
            if (isGameOver) return;
            
            isGameOver = true;
            IsInputBlocked = true;
            
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
