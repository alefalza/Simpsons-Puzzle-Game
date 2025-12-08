using System.Collections;
using System.Collections.Generic;
using GameModes.DonutStack.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace GameModes.DonutStack.Core
{
    public class PieceStack : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        
        private RectTransform rectTransform;
        private Canvas canvas;
        private Vector2 originalPos;
        private Renderer textRenderer;

        public int PieceCount => pieces.Count;
        public bool IsPlaced { get; private set; } = false;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Initialize()
        {
            canvas = HexGameManager.Instance.DragLayer.GetComponentInParent<Canvas>();
            
            int pieceCount = Random.Range(minStackHeight, maxStackHeight + 1);
        
            for (int i = 0; i < pieceCount; i++)
            {
                PieceColor color = availableColors[Random.Range(0, availableColors.Length)];
                Piece piece = Instantiate(piecePrefab, transform);
                piece.Initialize(color);
                AddPiece(piece);
            }
            
            ArrangePieces();
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
        
        public IEnumerator RemoveTopGroupWithDelay(int minCount, float delay)
        {
            int topCount = TopColorCount();
            
            if (topCount < minCount)
                yield break;

            PieceColor topColor = GetTopColor();

            for (int i = pieces.Count - 1; i >= 0; i--)
            {
                if (pieces[i].Color == topColor)
                {
                    Destroy(pieces[i].gameObject);
                    pieces.RemoveAt(i);
                    ArrangePieces();
                    yield return new WaitForSeconds(delay);
                }
                else break;
            }
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
        
        public int TopColorCount()
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

        #region Drag Events
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsPlaced) return;
            
            originalPos = rectTransform.anchoredPosition;
            transform.SetParent(HexGameManager.Instance.DragLayer, worldPositionStays: false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform,
                eventData.position,
                canvas.worldCamera,
                out var pos
            );

            rectTransform.anchoredPosition = pos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var cell = DetectCellUnderPointer(eventData);

            if (cell != null && !cell.IsOccupied)
            {
                HexGameManager.Instance.TryPlaceStack(cell, this);
            }
            else
            {
                transform.SetParent(HexGameManager.Instance.StackContainer, worldPositionStays: false);
                rectTransform.anchoredPosition = originalPos;
            }
        }
        
        private HexCell DetectCellUnderPointer(PointerEventData eventData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var r in results)
            {
                if (r.gameObject.TryGetComponent<HexCell>(out var cell))
                    return cell;
            }

            return null;
        }
        #endregion
    }
}
