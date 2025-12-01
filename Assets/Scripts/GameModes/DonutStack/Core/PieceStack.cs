using System.Collections.Generic;
using GameModes.DonutStack.Gameplay;
using TMPro;
using UnityEngine;

namespace GameModes.DonutStack.Core
{
    public class PieceStack : MonoBehaviour
    {
        [SerializeField] private Piece piecePrefab;
        [SerializeField] private float pieceSpacing = 0.15f;
        [SerializeField] private TextMeshPro topCountText;

        [SerializeField] private PieceColor[] availableColors = new PieceColor[] 
        { 
            PieceColor.Red, 
            PieceColor.Blue, 
            PieceColor.Green, 
            PieceColor.Yellow 
        };
        
        [SerializeField] private int minStackHeight = 1;
        [SerializeField] private int maxStackHeight = 4;
        
        private readonly List<Piece> pieces = new List<Piece>();
        
        private Camera mainCamera;
        private bool isDragging = false;
        private Vector3 originalPosition;
        private Vector3 dragOffset;
        private Renderer textRenderer;

        public int PieceCount => pieces.Count;
        public bool IsPlaced { get; private set; } = false;

        private void Awake()
        {
            mainCamera = Camera.main;
            UpdateTopCountText();
        }

        public void Initialize()
        {
            int pieceCount = Random.Range(minStackHeight, maxStackHeight + 1);
        
            for (int i = 0; i < pieceCount; i++)
            {
                PieceColor color = availableColors[Random.Range(0, availableColors.Length)];
                Piece piece = Instantiate(piecePrefab, transform);
                piece.Initialize(color);
                AddPiece(piece);
            }
        }

        public void AddPiece(Piece piece)
        {
            pieces.Add(piece);
            piece.transform.SetParent(transform);
        }

        public void ArrangePieces()
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                pieces[i].transform.localPosition = new Vector3(0, i * pieceSpacing, 0);
                pieces[i].SetSortingOrder(i);
            }

            UpdateTopCountText();
        }
        
        public PieceColor GetTopColor()
        {
            if (pieces.Count == 0) return PieceColor.None;
            
            return pieces[pieces.Count - 1].Color;
        }

        public List<Piece> RemovePiecesOfColor(PieceColor color)
        {
            List<Piece> removedPieces = new List<Piece>();

            for (int i = pieces.Count - 1; i >= 0; i--)
            {
                if (pieces[i].Color == color)
                {
                    removedPieces.Insert(0, pieces[i]);
                    pieces.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            return removedPieces;
        }

        public void PlaceOnCell(HexCell cell)
        {
            IsPlaced = true;
            transform.position = cell.transform.position;
        }
        
        private Sprite CreateCircleSprite()
        {
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            Vector2 center = new Vector2(32, 32);

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    pixels[y * 64 + x] = dist <= 30 ? Color.white : Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        }

        private void UpdateTopCountText()
        {
            int topCount = TopColorCount();

            if (topCount > 1)
            {
                topCountText.text = topCount.ToString();
                float posY = pieceSpacing * (pieces.Count - 1);
                topCountText.transform.localPosition = new Vector3(0, posY, -0.1f);
        
                textRenderer ??= topCountText.GetComponent<Renderer>();
                
                if (textRenderer != null)
                {
                    textRenderer.sortingLayerName = "HUD";
                    textRenderer.sortingOrder = 0;
                }
        
                topCountText.gameObject.SetActive(true);
            }
            else
            {
                topCountText.gameObject.SetActive(false);
            }
        }
        
        private int TopColorCount()
        {
            if (pieces.Count == 0) return 0;

            PieceColor topColor = GetTopColor();
            int count = 0;

            for (int i = pieces.Count - 1; i >= 0; i--)
            {
                if (pieces[i].Color == topColor)
                    count++;
                else
                    break;
            }

            return count;
        }
        
        private void SetDraggingSortingOrder(bool dragging)
        {
            int baseOrder = dragging ? 100 : 0;

            for (int i = 0; i < pieces.Count; i++)
            {
                pieces[i].SetSortingOrder(baseOrder + i);
            }
        }

        #region Mouse Events
        private void OnMouseDown()
        {
            if (!IsPlaced && HexGameManager.Instance != null && !HexGameManager.Instance.IsProcessingMatches)
            {
                isDragging = true;
                originalPosition = transform.position;

                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                dragOffset = transform.position - mouseWorldPos;

                SetDraggingSortingOrder(true);
            }
        }
        
        private void OnMouseDrag()
        {
            if (isDragging)
            {
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                transform.position = mouseWorldPos + dragOffset;
            }
        }

        private void OnMouseUp()
        {
            if (isDragging)
            {
                isDragging = false;

                HexCell targetCell = GetHexCellUnderMouse();
                bool placed = false;

                if (targetCell != null && !targetCell.IsOccupied && HexGameManager.Instance != null)
                {
                    HexGameManager.Instance.TryPlaceStack(targetCell, this);
                    placed = true;
                }

                if (!placed)
                {
                    transform.position = originalPosition;
                    SetDraggingSortingOrder(false);
                    ArrangePieces();
                }
                else
                {
                    SetDraggingSortingOrder(false);
                }
            }
        }
        
        private HexCell GetHexCellUnderMouse()
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != gameObject)
                {
                    HexCell cell = hit.collider.GetComponent<HexCell>();
                    if (cell != null)
                    {
                        return cell;
                    }
                }
            }

            return null;
        }
        #endregion
    }
}
