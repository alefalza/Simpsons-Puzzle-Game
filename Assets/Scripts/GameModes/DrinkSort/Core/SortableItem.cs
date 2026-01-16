using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModes.DrinkSort.Core
{
    public class SortableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private SortableItemType itemType = SortableItemType.None;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private Camera mainCamera;
        private Vector3 originalPosition;
        private Transform originalParent;
        private Tray currentTray;
        private bool isDragging = false;
        
        public SortableItemType ItemType => itemType;
        public Tray CurrentTray => currentTray;
        
        private void Awake()
        {
            mainCamera = Camera.main;
        }
        
        public void Initialize(SortableItemType type, Sprite sprite)
        {
            itemType = type;
            if (spriteRenderer != null && sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }
        
        public void SetTray(Tray tray)
        {
            currentTray = tray;
        }
        
        public void RemoveFromTray()
        {
            if (currentTray != null)
            {
                currentTray.RemoveItem(this);
                currentTray = null;
            }
        }
        
        #region Drag Events
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (currentTray == null) return;
            
            originalPosition = transform.position;
            originalParent = transform.parent;
            isDragging = true;
            
            // Asegurar que esté visible durante el arrastre
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 100;
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            transform.position = worldPos;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            isDragging = false;
            
            // Detectar bandeja bajo el puntero
            Tray targetTray = DetectTrayUnderPointer(eventData);
            
            if (targetTray != null && targetTray != currentTray && targetTray.CanAddItem())
            {
                // Mover a nueva bandeja
                if (currentTray != null)
                {
                    currentTray.RemoveItem(this);
                }
                
                targetTray.AddItem(this);
                currentTray = targetTray;
            }
            else
            {
                // Volver a posición original
                transform.position = originalPosition;
                transform.SetParent(originalParent);
            }
            
            // Restaurar sorting order
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 0;
            }
        }
        
        private Tray DetectTrayUnderPointer(PointerEventData eventData)
        {
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            foreach (var result in results)
            {
                Tray tray = result.gameObject.GetComponent<Tray>();
                if (tray != null)
                {
                    return tray;
                }
            }
            
            // Fallback: raycast físico
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            Collider2D hit = Physics2D.OverlapPoint(worldPos);
            if (hit != null)
            {
                return hit.GetComponent<Tray>();
            }
            
            return null;
        }
        #endregion
    }
}

