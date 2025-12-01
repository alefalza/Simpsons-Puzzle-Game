using GameModes.DonutStack.Core;
using UnityEngine;

namespace GameModes.DonutStack.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class HexCell : MonoBehaviour
    {
        [SerializeField] private Color normalColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.5f, 0.7f);

        private SpriteRenderer spriteRenderer;
    
        public Vector2Int AxialCoords { get; private set; }
        public PieceStack Stack { get; private set; }
        public bool IsOccupied => Stack != null;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = CreateHexagonSprite();
            }
        
            spriteRenderer.color = normalColor;
        
            PolygonCollider2D coll = GetComponent<PolygonCollider2D>();
        
            if (coll.points.Length == 0)
            {
                SetupHexagonCollider(coll);
            }
        }

        public void Initialize(Vector2Int coords)
        {
            AxialCoords = coords;
        }
        
        public void SetStack(PieceStack newStack)
        {
            Stack = newStack;
            newStack.PlaceOnCell(this);
        }

        public void ClearStack()
        {
            Stack = null;
        }

        private void SetupHexagonCollider(PolygonCollider2D coll)
        {
            Vector2[] points = new Vector2[6];
        
            for (int i = 0; i < 6; i++)
            {
                float angle = 60f * i * Mathf.Deg2Rad;
                points[i] = new Vector2(Mathf.Cos(angle) * 0.5f, Mathf.Sin(angle) * 0.5f);
            }
        
            coll.points = points;
        }

        #region Drawing Methods
        private Sprite CreateHexagonSprite()
        {
            Texture2D texture = new Texture2D(128, 128);
            Color[] pixels = new Color[128 * 128];
        
            Vector2 center = new Vector2(64, 64);
            float radius = 60;
        
            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    Vector2 point = new Vector2(x, y);
                
                    if (IsInsideHexagon(point, center, radius))
                    {
                        pixels[y * 128 + x] = Color.white;
                    }
                    else
                    {
                        pixels[y * 128 + x] = Color.clear;
                    }
                }
            }
        
            texture.SetPixels(pixels);
            texture.Apply();
        
            return Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
        }

        private bool IsInsideHexagon(Vector2 point, Vector2 center, float radius)
        {
            Vector2 diff = point - center;
            float angle = Mathf.Atan2(diff.y, diff.x);
            float hexAngle = Mathf.PI / 3f;
        
            float sectionAngle = Mathf.Round(angle / hexAngle) * hexAngle;
            Vector2 sectionPoint = center + new Vector2(Mathf.Cos(sectionAngle), Mathf.Sin(sectionAngle)) * radius;
        
            return Vector2.Distance(point, center) <= Vector2.Distance(sectionPoint, center);
        }
        #endregion
        
        #region Mouse Events
        private void OnMouseEnter()
        {
            if (!IsOccupied && HexGameManager.Instance != null && !HexGameManager.Instance.IsProcessingMatches)
            {
                spriteRenderer.color = hoverColor;
            }
        }

        private void OnMouseExit()
        {
            spriteRenderer.color = normalColor;
        }
        #endregion
    }
}
