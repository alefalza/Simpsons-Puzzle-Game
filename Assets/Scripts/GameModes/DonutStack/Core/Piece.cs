using UnityEngine;

namespace GameModes.DonutStack.Core
{
    [System.Serializable]
    public enum PieceColor
    {
        None,
        Red,
        Blue,
        Green,
        Yellow,
        White,
        Gray,
        Magenta
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class Piece : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
    
        private static readonly Color[] colorMap = new Color[]
        {
            UnityEngine.Color.clear,
            new Color(1f, 0.2f, 0.2f),
            new Color(0.2f, 0.4f, 1f),
            new Color(0.3f, 1f, 0.3f),
            new Color(1f, 1f, 0.2f),
            new Color(0.95f, 0.95f, 0.95f),
            new Color(0.5f, 0.5f, 0.5f),
            new Color(1f, 0.3f, 1f)
        };
    
        public PieceColor Color { get; private set; }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = CreateHexagonSprite();
            }
        }

        public void Initialize(PieceColor color)
        {
            Color = color;
            SetSpriteColor();
        }
    
        public void SetSortingOrder(int order)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = order;
            }
        }

        #region Drawing Methods
        private void SetSpriteColor()
        {
            if (spriteRenderer != null && (int)Color < colorMap.Length)
            {
                spriteRenderer.color = colorMap[(int)Color];
            }
        }
        
        private Sprite CreateHexagonSprite()
        {
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
        
            Vector2 center = new Vector2(32, 32);
            float radius = 28;
        
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    Vector2 point = new Vector2(x, y);
                
                    if (IsInsideHexagon(point, center, radius))
                    {
                        float dist = DistanceToHexagonEdge(point, center, radius);
                        
                        if (dist > 2f)
                        {
                            pixels[y * 64 + x] = UnityEngine.Color.white;
                        }
                        else
                        {
                            pixels[y * 64 + x] = new Color(0.3f, 0.3f, 0.3f, 1f);
                        }
                    }
                    else
                    {
                        pixels[y * 64 + x] = UnityEngine.Color.clear;
                    }
                }
            }
        
            texture.SetPixels(pixels);
            texture.Apply();
        
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        }

        private bool IsInsideHexagon(Vector2 point, Vector2 center, float radius)
        {
            Vector2 diff = point - center;
            float q = (Mathf.Sqrt(3f) / 3f * diff.x - 1f / 3f * diff.y) / radius;
            float r = (2f / 3f * diff.y) / radius;
        
            return Mathf.Abs(q) <= 1f && Mathf.Abs(r) <= 1f && Mathf.Abs(q + r) <= 1f;
        }

        private float DistanceToHexagonEdge(Vector2 point, Vector2 center, float radius)
        {
            Vector2 diff = point - center;
            float angle = Mathf.Atan2(diff.y, diff.x);
            float hexAngle = Mathf.PI / 3f;
            float section = Mathf.Round(angle / hexAngle);
            float edgeAngle = section * hexAngle;
        
            float distToEdge = radius / Mathf.Cos(angle - edgeAngle);
            float actualDist = diff.magnitude;
        
            return distToEdge - actualDist;
        }
        #endregion
    }
}
