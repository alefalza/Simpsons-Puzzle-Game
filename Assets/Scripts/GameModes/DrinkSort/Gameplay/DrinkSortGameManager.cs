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
        [SerializeField] private int gridWidth = 4;
        [SerializeField] private int gridHeight = 4;
        
        [Header("Item Settings")]
        [SerializeField] private ItemReserve itemReserve; // Para obtener sprites y prefab
        [SerializeField] private Transform itemsContainer;
        [SerializeField] private int initialItemsPerTray = 2;
        [SerializeField] private int itemsToFillOnClear = 3;
        
        [Header("Tray Reserve Settings")]
        [SerializeField] private TrayReserve.ItemData[] trayReserveItems; // Items disponibles para las reservas de bandejas
        [SerializeField] private int initialTrayReserveSize = 20;
        
        [Header("Game Settings")]
        [SerializeField] private float timeLimit = 120f; // 2 minutos por defecto
        
        private float currentTime;
        private int totalItemsUsed = 0;
        private int score = 0;
        private bool isGameOver = false;
        
        private DrinkSortHUDController DrinkSortHUDController => hudController as DrinkSortHUDController;
        
        public float TimeRemaining => Mathf.Max(0, timeLimit - currentTime);
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
            
            // Inicializar grilla
            trayGrid.Initialize(gridWidth, gridHeight);
            
            // Inicializar reservas de cada bandeja
            InitializeTrayReserves();
            
            // Poblar bandejas inicialmente
            StartCoroutine(PopulateInitialTrays());
            
            // Actualizar HUD
            UpdateHUD();
        }
        
        private void InitializeTrayReserves()
        {
            foreach (var tray in trayGrid.Trays.Values)
            {
                tray.InitializeReserve(trayReserveItems, initialTrayReserveSize);
            }
        }
        
        private IEnumerator PopulateInitialTrays()
        {
            yield return new WaitForSeconds(GameConstants.DrinkSort.InitialPopulateDelay);
            
            List<Tray> emptyTrays = trayGrid.GetEmptyTrays();
            
            foreach (var tray in emptyTrays)
            {
                int itemsToAdd = Random.Range(1, initialItemsPerTray + 1);
                
                tray.PopulateFromReserve(
                    itemsToAdd,
                    itemReserve != null ? itemReserve.ItemPrefab : null,
                    GetSpriteForType,
                    itemsContainer != null ? itemsContainer : transform
                );
                
                yield return new WaitForSeconds(GameConstants.DrinkSort.ItemPopulateDelay);
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
            
            // Agregar puntuación
            AddScore(GameConstants.DrinkSort.ScorePerMatch);
            
            // Limpiar bandeja
            tray.ClearItems();
            
            yield return new WaitForSeconds(GameConstants.DrinkSort.MatchProcessDelay);
            
            // Poblar bandeja desde su propia reserva
            if (tray.Reserve.HasItems() && itemReserve != null)
            {
                tray.PopulateFromReserve(
                    itemsToFillOnClear,
                    itemReserve.ItemPrefab,
                    GetSpriteForType,
                    itemsContainer != null ? itemsContainer : transform
                );
                
                totalItemsUsed += itemsToFillOnClear;
            }
            
            yield return new WaitForSeconds(GameConstants.DrinkSort.PostPopulateDelay);
            
            IsInputBlocked = false;
            
            // Actualizar HUD
            UpdateHUD();
            
            // Verificar condición de victoria
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
            // Victoria: usar todos los elementos disponibles de todas las reservas
            // y que no queden items en las bandejas
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
                
                // Si todas las reservas están vacías y no hay items en las bandejas, ganaste
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
