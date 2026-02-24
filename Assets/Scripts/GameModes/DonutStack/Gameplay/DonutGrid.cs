using System.Collections.Generic;
using UnityEngine;

namespace GameModes.DonutStack.Gameplay
{
    public class DonutGrid : MonoBehaviour
    {
        [SerializeField] private GridCell gridCellPrefab;
        [SerializeField] private float hexSize = 1f;
        
        private readonly Vector2Int[] hexDirections = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1)
        };
        
        private readonly Dictionary<Vector2Int, GridCell> cells = new Dictionary<Vector2Int, GridCell>();

        public void Initialize(int radius)
        {
            for (int q = -radius; q <= radius; q++)
            {
                int r1 = Mathf.Max(-radius, -q - radius);
                int r2 = Mathf.Min(radius, -q + radius);
            
                for (int r = r1; r <= r2; r++)
                {
                    CreateHexCell(q, r);
                }
            }
        }
        
        public List<GridCell> GetNeighbours(GridCell cell)
        {
            List<GridCell> neighbours = new List<GridCell>();
        
            foreach (var direction in hexDirections)
            {
                Vector2Int neighbourCoords = cell.AxialCoords + direction;
            
                if (cells.TryGetValue(neighbourCoords, out GridCell neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }
        
            return neighbours;
        }

        public GridCell GetCell(Vector2Int coords)
        {
            cells.TryGetValue(coords, out GridCell cell);
        
            return cell;
        }

        public bool HasEmptyCells()
        {
            foreach (var cell in cells.Values)
            {
                if (!cell.IsOccupied)
                {
                    return true;
                }
            }
        
            return false;
        }

        private void CreateHexCell(int q, int r)
        {
            Vector2Int axialCoords = new Vector2Int(q, r);
        
            var cell = Instantiate(gridCellPrefab, transform);
            cell.name = $"HexCell_{q}_{r}";
            cell.Initialize(axialCoords);
            cell.GetComponent<RectTransform>().anchoredPosition = AxialToUI(axialCoords);
        
            cells[axialCoords] = cell;
        }

        private Vector2 AxialToUI(Vector2Int axial)
        {
            float x = hexSize * (Mathf.Sqrt(3) * axial.x + Mathf.Sqrt(3) / 2f * axial.y);
            float y = hexSize * (3f / 2f * axial.y);

            return new Vector2(x, y);
        }
    }
}
