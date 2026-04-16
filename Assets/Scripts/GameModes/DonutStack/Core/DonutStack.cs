using System;
using System.Collections;
using System.Collections.Generic;
using GameModes.DonutStack.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace GameModes.DonutStack.Core
{
    public class DonutStack : DraggableItem
    {
        [SerializeField] private Donut donutPrefab;
        [SerializeField] private int minStackHeight = 1;
        [SerializeField] private int maxStackHeight = 4;
        [SerializeField] private float pieceSpacing = 0.15f;
        [SerializeField] private TextMeshPro topCountText;
        
        private readonly List<Donut> pieces = new List<Donut>();
        
        private Canvas canvas;
        private Renderer textRenderer;
        private StackSlot parentSlot;

        public int PieceCount => pieces.Count;
        public bool IsPlaced { get; private set; } = false;

        private void Awake()
        {
            textRenderer = topCountText.GetComponent<Renderer>();
            textRenderer.sortingLayerName = "HUD";
            textRenderer.sortingOrder = 0;
        }

        public void Initialize(StackSlot slot, Func<DonutColor, Sprite> getSpriteFunc, Func<DonutColor> getRandomColorFunc)
        {
            canvas = DonutStackGameManager.Instance.DragLayer.GetComponentInParent<Canvas>();
            
            parentSlot = slot;
            parentSlot.SetOccupied(true);

            InstantiatePieces(getSpriteFunc, getRandomColorFunc);
            ArrangePieces();
        }

        private void InstantiatePieces(Func<DonutColor, Sprite> getSpriteFunc, Func<DonutColor> getRandomColorFunc)
        {
            int pieceCount = Random.Range(minStackHeight, maxStackHeight + 1);
        
            for (int i = 0; i < pieceCount; i++)
            {
                DonutColor color = getRandomColorFunc.Invoke();
                Donut donut = Instantiate(donutPrefab, transform);
                Sprite sprite = getSpriteFunc(color);
                donut.Initialize(color, sprite);
                AddPiece(donut);
            }
        }
        
        public void AddPiece(Donut donut)
        {
            pieces.Add(donut);
            donut.transform.SetParent(transform);
        }

        public void ArrangePieces()
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                pieces[i].transform.localPosition = new Vector3(0, i * pieceSpacing, 0);
            }

            UpdateTopCountText();
        }
        
        private void UpdateTopCountText()
        {
            int topCount = TopColorCount();

            if (topCount > 1)
            {
                topCountText.text = topCount.ToString();
                float posY = pieceSpacing * (pieces.Count - 1);
                topCountText.transform.localPosition = new Vector3(0, posY, -0.1f);
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

            DonutColor topColor = GetTopColor();
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
        
        public DonutColor GetTopColor()
        {
            if (pieces.Count == 0) return DonutColor.None;
            
            return pieces[pieces.Count - 1].Color;
        }

        public List<Donut> RemovePiecesOfColor(DonutColor color)
        {
            List<Donut> removedPieces = new List<Donut>();

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

        public void PlaceOnCell(GridCell cell)
        {
            IsPlaced = true;
            transform.position = cell.transform.position;
        }
        
        public IEnumerator RemoveTopGroupWithDelay(int minCount, float delay)
        {
            int topCount = TopColorCount();
            
            if (topCount < minCount)
                yield break;

            DonutColor topColor = GetTopColor();

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
        
        #region Drag Events
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (IsPlaced) return;
            
            base.OnBeginDrag(eventData);
            
            transform.SetParent(DonutStackGameManager.Instance.DragLayer, worldPositionStays: false);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            var dragLayerRect = (RectTransform)DonutStackGameManager.Instance.DragLayer;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    dragLayerRect,
                    eventData.position,
                    canvas.worldCamera,
                    out var worldPos))
            {
                transform.position = worldPos;
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            
            var cell = DetectCellUnderPointer(eventData);

            if (cell != null && !cell.IsOccupied)
            {
                DonutStackGameManager.Instance.TryPlaceStack(cell, this);
                parentSlot.SetOccupied(false);
            }
            else
            {
                transform.SetParent(originalParent, worldPositionStays: false);
                transform.position = originalPosition;
            }
        }
        
        private GridCell DetectCellUnderPointer(PointerEventData eventData)
        {
            return DetectObjectUnderPointer<GridCell>(eventData);
        }
        
        #endregion
    }
}
