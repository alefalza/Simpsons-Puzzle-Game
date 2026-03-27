using System;
using System.Collections.Generic;
using GameModes.DrinkSort.Core;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameModes.DrinkSort.Gameplay
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class TrayGrid : MonoBehaviour
    {
        [SerializeField] private Tray trayPrefab;
        
        private GridLayoutGroup gridLayoutGroup;
        private int gridWidth;
        private int gridHeight;
        private int trayPopulationPercent;
        private SortableItem itemPrefab;
        private Func<SortableItemType, Color> getColorFunc;
        private Func<SortableItemType> getRandomTypeFunc;
        private Func<int, List<SortableItemType>> buildSpawnPoolFunc;

        public Dictionary<Vector2Int, Tray> Trays { get; } = new Dictionary<Vector2Int, Tray>();

        private void Awake()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            
            if (gridLayoutGroup == null)
            {
                Debug.LogError("[TrayGrid] GridLayoutGroup component is required!");
            }
        }
        
        public void Initialize(
            int width,
            int height,
            int trayPopulationPercent,
            SortableItem prefab,
            Func<SortableItemType, Color> getColorFunc,
            Func<SortableItemType> getRandomTypeFunc,
            Func<int, List<SortableItemType>> buildSpawnPoolFunc = null
        )
        {
            gridWidth = width;
            gridHeight = height;
            this.trayPopulationPercent = Mathf.Clamp(trayPopulationPercent, 0, 100);
            itemPrefab = prefab;
            this.getColorFunc = getColorFunc;
            this.getRandomTypeFunc = getRandomTypeFunc;
            this.buildSpawnPoolFunc = buildSpawnPoolFunc;
            ClearGrid();
            CreateGrid();
        }
        
        private void ClearGrid()
        {
            foreach (var tray in Trays.Values)
            {
                if (tray != null)
                {
                    Destroy(tray.gameObject);
                }
            }
            
            Trays.Clear();
        }
        
        private void CreateGrid()
        {
            if (gridLayoutGroup == null)
            {
                Debug.LogError("[TrayGrid] GridLayoutGroup is null!");
                return;
            }
            
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = gridWidth;
            
            int index = 0;
            
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    
                    Tray tray = Instantiate(trayPrefab, transform);
                    tray.Initialize(gridPos, index);
                    
                    Trays[gridPos] = tray;
                    index++;
                }
            }

            PopulateInitialItems();
        }

        private void PopulateInitialItems()
        {
            if (itemPrefab == null || getColorFunc == null || getRandomTypeFunc == null || Trays.Count == 0)
            {
                return;
            }

            int totalCapacity = Trays.Count * 3;
            int targetItems = Mathf.RoundToInt(totalCapacity * (trayPopulationPercent / 100f));
            int totalItemsToSpawn = targetItems - (targetItems % 3);

            if (totalItemsToSpawn <= 0)
            {
                return;
            }

            List<SortableItemType> spawnPool = buildSpawnPoolFunc != null
                ? buildSpawnPoolFunc.Invoke(totalItemsToSpawn)
                : BuildSpawnPool(totalItemsToSpawn);

            Shuffle(spawnPool);

            foreach (var itemType in spawnPool)
            {
                List<Tray> traysWithSpace = GetTraysWithSpace();
                if (traysWithSpace.Count == 0)
                {
                    break;
                }

                Tray randomTray = traysWithSpace[Random.Range(0, traysWithSpace.Count)];
                randomTray.TrySpawnInitialItem(itemPrefab, itemType, getColorFunc);
            }
        }

        private List<SortableItemType> BuildSpawnPool(int totalItemsToSpawn)
        {
            List<SortableItemType> pool = new List<SortableItemType>(totalItemsToSpawn);
            int groups = totalItemsToSpawn / 3;

            for (int i = 0; i < groups; i++)
            {
                SortableItemType type = GetValidRandomType();
                if (type == SortableItemType.None)
                {
                    break;
                }

                pool.Add(type);
                pool.Add(type);
                pool.Add(type);
            }

            return pool;
        }

        private SortableItemType GetValidRandomType()
        {
            const int maxAttempts = 20;

            for (int i = 0; i < maxAttempts; i++)
            {
                SortableItemType randomType = getRandomTypeFunc.Invoke();
                if (randomType != SortableItemType.None)
                {
                    return randomType;
                }
            }

            return SortableItemType.None;
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
        
        #region Helpers
        public Tray GetTrayByIndex(int index)
        {
            int count = 0;
            
            foreach (var tray in Trays.Values)
            {
                if (count == index)
                    return tray;
                
                count++;
            }
            
            return null;
        }
        
        public Tray GetTray(Vector2Int position)
        {
            Trays.TryGetValue(position, out Tray tray);
            
            return tray;
        }
        
        public List<Tray> GetEmptyTrays()
        {
            List<Tray> emptyTrays = new List<Tray>();
            
            foreach (var tray in Trays.Values)
            {
                if (tray.IsEmpty)
                {
                    emptyTrays.Add(tray);
                }
            }
            
            return emptyTrays;
        }
        
        public List<Tray> GetTraysWithSpace()
        {
            List<Tray> traysWithSpace = new List<Tray>();
            
            foreach (var tray in Trays.Values)
            {
                if (tray.CanAddItem())
                {
                    traysWithSpace.Add(tray);
                }
            }
            
            return traysWithSpace;
        }
        
        public bool HasEmptyTrays()
        {
            foreach (var tray in Trays.Values)
            {
                if (tray.IsEmpty)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public int GetEmptyTrayCount()
        {
            int count = 0;
            
            foreach (var tray in Trays.Values)
            {
                if (tray.IsEmpty)
                {
                    count++;
                }
            }
            
            return count;
        }
        #endregion
    }
}
