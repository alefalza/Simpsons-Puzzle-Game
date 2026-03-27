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
        [Header("Grid Settings")]
        [SerializeField] private TrayGrid trayGrid;
        
        [Header("Item Settings")]
        [SerializeField] private ItemReserve itemReserve;

        [Header("Game Settings")]
        [SerializeField] private Transform dragLayer;

        protected override string GameModeName => "DrinkSort";

        private float currentTime;
        private bool isGameOver = false;
        
        private DrinkSortHUDController DrinkSortHUDController => hudController as DrinkSortHUDController;
        private readonly List<WeightedType> weightedLevelTypes = new List<WeightedType>();
        
        // Properties that get values from levelData or default values
        private int GridWidth => levelData != null ? ((DrinkSortLevelDefinition)levelData).gridWidth : 4;
        private int GridHeight => levelData != null ? ((DrinkSortLevelDefinition)levelData).gridHeight : 4;
        private int TrayPopulationPercent => levelData != null ? ((DrinkSortLevelDefinition)levelData).trayPopulationPercent : 66;
        private float TimeLimit => levelData != null ? ((DrinkSortLevelDefinition)levelData).timeLimit : 120f;
        private float MatchProcessDelay => levelData != null ? ((DrinkSortLevelDefinition)levelData).matchProcessDelay : GameConstants.DrinkSort.MatchProcessDelay;
        private DrinkSortLevelDefinition DrinkSortLevelData => levelData as DrinkSortLevelDefinition;

        public Transform DragLayer => dragLayer;
        public float TimeRemaining => Mathf.Max(0, TimeLimit - currentTime);
        public int ItemsRemaining => GetTotalItemCount();
        
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
            isGameOver = false;
            ConfigureLevelTypeWeights();
            
            trayGrid.Initialize(
                GridWidth, 
                GridHeight,
                TrayPopulationPercent,
                itemReserve != null ? itemReserve.ItemPrefab : null,
                GetColorForType,
                GetRandomWeightedLevelType,
                BuildInitialSpawnPoolByWeights
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
        
        private void ConfigureLevelTypeWeights()
        {
            weightedLevelTypes.Clear();

            if (itemReserve == null)
            {
                return;
            }

            List<SortableItemType> availableTypes = itemReserve.GetAvailableTypes();
            if (availableTypes.Count == 0)
            {
                return;
            }

            int positiveWeightCount = 0;

            foreach (SortableItemType type in availableTypes)
            {
                int weight = DrinkSortLevelData != null ? DrinkSortLevelData.GetSpawnWeight(type) : 1;
                if (weight > 0)
                {
                    positiveWeightCount++;
                }

                weightedLevelTypes.Add(new WeightedType(type, weight));
            }

            // Fallback: if all configured weights are 0, use uniform weight 1.
            if (positiveWeightCount == 0)
            {
                weightedLevelTypes.Clear();
                foreach (SortableItemType type in availableTypes)
                {
                    weightedLevelTypes.Add(new WeightedType(type, 1));
                }
            }
        }

        private SortableItemType GetRandomWeightedLevelType()
        {
            if (weightedLevelTypes.Count == 0)
            {
                return SortableItemType.None;
            }

            int totalWeight = 0;
            foreach (WeightedType weightedType in weightedLevelTypes)
            {
                totalWeight += Mathf.Max(0, weightedType.Weight);
            }

            if (totalWeight <= 0)
            {
                return SortableItemType.None;
            }

            int randomValue = Random.Range(0, totalWeight);
            foreach (WeightedType weightedType in weightedLevelTypes)
            {
                int weight = Mathf.Max(0, weightedType.Weight);
                if (weight == 0) continue;

                if (randomValue < weight)
                {
                    return weightedType.Type;
                }

                randomValue -= weight;
            }

            return weightedLevelTypes[weightedLevelTypes.Count - 1].Type;
        }

        private List<SortableItemType> BuildInitialSpawnPoolByWeights(int totalItemsToSpawn)
        {
            // We spawn in groups of 3 identical items to guarantee matchability.
            if (totalItemsToSpawn <= 0)
            {
                return new List<SortableItemType>();
            }

            int totalGroups = totalItemsToSpawn / 3;
            if (totalGroups <= 0)
            {
                return new List<SortableItemType>();
            }

            // Use the configured weights as a proportional target distribution (e.g. Red=30 means ~30% of groups).
            List<WeightedType> types = new List<WeightedType>(weightedLevelTypes.Count);
            int totalWeight = 0;

            foreach (var wt in weightedLevelTypes)
            {
                int w = Mathf.Max(0, wt.Weight);
                if (wt.Type == SortableItemType.None || w <= 0) continue;
                types.Add(new WeightedType(wt.Type, w));
                totalWeight += w;
            }

            if (types.Count == 0 || totalWeight <= 0)
            {
                // Fallback: uniform distribution via existing random picker (still grouped by 3).
                return BuildInitialSpawnPoolUniform(totalItemsToSpawn);
            }

            int[] baseGroups = new int[types.Count];
            float[] remainders = new float[types.Count];
            int assigned = 0;

            for (int i = 0; i < types.Count; i++)
            {
                float exact = (float)totalGroups * types[i].Weight / totalWeight;
                int g = Mathf.FloorToInt(exact);
                baseGroups[i] = Mathf.Max(0, g);
                assigned += baseGroups[i];
                remainders[i] = exact - g;
            }

            int remaining = totalGroups - assigned;
            while (remaining > 0)
            {
                int bestIndex = 0;
                float bestRemainder = float.MinValue;

                for (int i = 0; i < remainders.Length; i++)
                {
                    // Deterministic tie-breaker: higher remainder first, then higher weight.
                    if (remainders[i] > bestRemainder || (Mathf.Approximately(remainders[i], bestRemainder) && types[i].Weight > types[bestIndex].Weight))
                    {
                        bestRemainder = remainders[i];
                        bestIndex = i;
                    }
                }

                baseGroups[bestIndex]++;
                // Set to -1 so it won't be picked again unless needed after others.
                remainders[bestIndex] = -1f;
                remaining--;
            }

            List<SortableItemType> pool = new List<SortableItemType>(totalGroups * 3);
            for (int i = 0; i < types.Count; i++)
            {
                for (int g = 0; g < baseGroups[i]; g++)
                {
                    pool.Add(types[i].Type);
                    pool.Add(types[i].Type);
                    pool.Add(types[i].Type);
                }
            }

            // If any rounding edge-case ever happens, pad with valid random groups.
            while (pool.Count < totalItemsToSpawn)
            {
                SortableItemType t = GetRandomWeightedLevelType();
                if (t == SortableItemType.None) break;
                pool.Add(t);
                pool.Add(t);
                pool.Add(t);
            }

            // Trim (shouldn't be needed, but keeps invariants).
            if (pool.Count > totalItemsToSpawn)
            {
                pool.RemoveRange(totalItemsToSpawn, pool.Count - totalItemsToSpawn);
            }

            return pool;
        }

        private List<SortableItemType> BuildInitialSpawnPoolUniform(int totalItemsToSpawn)
        {
            List<SortableItemType> pool = new List<SortableItemType>(totalItemsToSpawn);
            int groups = totalItemsToSpawn / 3;
            for (int i = 0; i < groups; i++)
            {
                SortableItemType t = GetRandomWeightedLevelType();
                if (t == SortableItemType.None) break;
                pool.Add(t);
                pool.Add(t);
                pool.Add(t);
            }
            return pool;
        }

        private readonly struct WeightedType
        {
            public WeightedType(SortableItemType type, int weight)
            {
                Type = type;
                Weight = weight;
            }

            public SortableItemType Type { get; }
            public int Weight { get; }
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
                DrinkSortHUDController.SetLevelText(currentLevelNumber);
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
                hudController.ShowWinPopup(0);
            }
        }
        
        private void OnGameOver()
        {
            if (isGameOver) return;
            
            isGameOver = true;
            IsInputBlocked = true;
            
            if (hudController != null)
            {
                hudController.ShowGameOverOverlay(0);
            }
        }
    }
}
