using System.Collections.Generic;
using GameModes.DrinkSort.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModes.DrinkSort.Core
{
    public class SortableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private SortableItemType itemType = SortableItemType.None;
        [SerializeField] private Image image;
        
        private Camera mainCamera;
        private Vector3 originalPosition;
        private Transform originalParent;
        private Tray parentTray;
        private bool isDragging = false;
        
        public SortableItemType ItemType => itemType;
        public Tray ParentTray => parentTray;
        
        private void Awake()
        {
            mainCamera = Camera.main;
        }
        
        public void Initialize(SortableItemType type, Color color)
        {
            itemType = type;
            
            if (image != null)
            {
                image.color = color;
            }
        }
        
        public void SetTray(Tray tray)
        {
            parentTray = tray;
        }
        
        public void RemoveFromTray()
        {
            if (parentTray != null)
            {
                parentTray.RemoveItem(this);
                parentTray = null;
            }
        }
        
        #region Drag Events
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (parentTray == null) return;
            
            originalPosition = transform.position;
            originalParent = transform.parent;
            isDragging = true;
            transform.SetParent(DrinkSortGameManager.Instance.DragLayer, worldPositionStays: false);
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
            
            // Detect tray under pointer
            Tray targetTray = DetectTrayUnderPointer(eventData);
            
            if (targetTray != null && targetTray.CanAddItem())
            {
                // Get mouse position to find the closest slot
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
                worldPos.z = 0;
                int slotIndex = targetTray.GetSlotIndexForPosition(worldPos);
                
                if (slotIndex == -1)
                {
                    // If no slot is found, return to the original position
                    transform.position = originalPosition;
                    transform.SetParent(originalParent);
                    return;
                }
                
                // If it's the same tray, verify that the target slot is free or different
                if (targetTray == parentTray)
                {
                    if (targetTray.IsSlotFree(slotIndex))
                    {
                        // Remove from the current slot and add to the new slot
                        targetTray.RemoveItem(this);
                        targetTray.AddItem(this, slotIndex);
                    }
                    else
                    {
                        // Slot is occupied, return to the original position
                        transform.position = originalPosition;
                        transform.SetParent(originalParent);
                    }
                }
                else
                {
                    // Move to the new tray
                    if (parentTray != null)
                    {
                        parentTray.RemoveItem(this);
                    }
                    
                    if (targetTray.AddItem(this, slotIndex))
                    {
                        parentTray = targetTray;
                    }
                    else
                    {
                        // Could not be added, return to the original position
                        transform.position = originalPosition;
                        transform.SetParent(originalParent);
                    }
                }
            }
            else
            {
                // Return to original position
                transform.position = originalPosition;
                transform.SetParent(originalParent);
            }
        }
        
        private Tray DetectTrayUnderPointer(PointerEventData eventData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            foreach (var result in results)
            {
                Tray tray = result.gameObject.GetComponent<Tray>();
                
                if (tray != null)
                {
                    return tray;
                }
            }
            
            // Fallback: physics raycast
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
