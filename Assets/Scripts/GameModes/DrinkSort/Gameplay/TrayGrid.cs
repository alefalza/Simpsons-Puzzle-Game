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
        [SerializeField] private int gridWidth = 4;
        [SerializeField] private int gridHeight = 4;
        
        private GridLayoutGroup gridLayoutGroup;
        private readonly Dictionary<Vector2Int, Tray> trays = new Dictionary<Vector2Int, Tray>();
        
        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public int TotalTrays => gridWidth * gridHeight;
        public Dictionary<Vector2Int, Tray> Trays => trays;
        
        private void Awake()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            
            if (gridLayoutGroup == null)
            {
                Debug.LogError("[TrayGrid] GridLayoutGroup component is required!");
            }
        }
        
        public void Initialize(int width, int height)
        {
            gridWidth = width;
            gridHeight = height;
            
            ClearGrid();
            CreateGrid();
        }
        
        private void CreateGrid()
        {
            if (gridLayoutGroup == null)
            {
                Debug.LogError("[TrayGrid] GridLayoutGroup is null!");
                return;
            }
            
            // Configurar GridLayoutGroup
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = gridWidth;
            
            // Crear bandejas
            int index = 0;
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    
                    Tray tray = Instantiate(trayPrefab, transform);
                    tray.Initialize(gridPos, index);
                    
                    trays[gridPos] = tray;
                    index++;
                }
            }
        }
        
        public Tray GetTrayByIndex(int index)
        {
            int count = 0;
            foreach (var tray in trays.Values)
            {
                if (count == index)
                    return tray;
                count++;
            }
            return null;
        }
        
        public Tray GetTray(Vector2Int position)
        {
            trays.TryGetValue(position, out Tray tray);
            return tray;
        }
        
        public List<Tray> GetEmptyTrays()
        {
            List<Tray> emptyTrays = new List<Tray>();
            
            foreach (var tray in trays.Values)
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
            
            foreach (var tray in trays.Values)
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
            foreach (var tray in trays.Values)
            {
                if (tray.IsEmpty)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        private void ClearGrid()
        {
            foreach (var tray in trays.Values)
            {
                if (tray != null)
                {
                    Destroy(tray.gameObject);
                }
            }
            
            trays.Clear();
        }
        
        public int GetEmptyTrayCount()
        {
            int count = 0;
            foreach (var tray in trays.Values)
            {
                if (tray.IsEmpty)
                {
                    count++;
                }
            }
            return count;
        }
    }
}

