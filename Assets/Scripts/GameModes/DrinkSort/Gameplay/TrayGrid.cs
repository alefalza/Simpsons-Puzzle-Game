using System;
using System.Collections.Generic;
using GameModes.DrinkSort.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GameModes.DrinkSort.Gameplay
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class TrayGrid : MonoBehaviour
    {
        [SerializeField] private Tray trayPrefab;
        
        private GridLayoutGroup gridLayoutGroup;
        private int gridWidth;
        private int gridHeight;
        private SortableItem itemPrefab;
        private Func<SortableItemType, Color> getColorFunc;
        private Func<SortableItemType> getRandomTypeFunc;

        public Dictionary<Vector2Int, Tray> Trays { get; } = new Dictionary<Vector2Int, Tray>();

        private void Awake()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            
            if (gridLayoutGroup == null)
            {
                Debug.LogError("[TrayGrid] GridLayoutGroup component is required!");
            }
        }
        
        public void Initialize(int width, int height, SortableItem prefab, Func<SortableItemType, Color> getColorFunc, Func<SortableItemType> getRandomTypeFunc)
        {
            gridWidth = width;
            gridHeight = height;
            itemPrefab = prefab;
            this.getColorFunc = getColorFunc;
            this.getRandomTypeFunc = getRandomTypeFunc;
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
                    tray.Initialize(gridPos, index, itemPrefab, getColorFunc, getRandomTypeFunc);
                    
                    Trays[gridPos] = tray;
                    index++;
                }
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
