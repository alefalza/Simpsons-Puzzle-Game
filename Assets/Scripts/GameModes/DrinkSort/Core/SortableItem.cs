using GameModes.DrinkSort.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModes.DrinkSort.Core
{
    public class SortableItem : DraggableItem
    {
        [SerializeField] private Image image;

        private Camera mainCamera;

        public SortableItemType ItemType { get; private set; }
        public Tray ParentTray { get; private set; }
        
        private void Awake()
        {
            mainCamera = Camera.main;
        }
        
        public void Initialize(SortableItemType type, Sprite sprite)
        {
            ItemType = type;

            if (sprite != null)
            {
                image.sprite = sprite;
            }
        }
        
        public void SetTray(Tray tray)
        {
            ParentTray = tray;
        }
        
        public void RemoveFromTray()
        {
            if (ParentTray != null)
            {
                ParentTray.RemoveItem(this);
                ParentTray = null;
            }
        }
        
        #region Drag Events
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (ParentTray == null) return;
            
            base.OnBeginDrag(eventData);
            
            transform.SetParent(DrinkSortGameManager.Instance.DragLayer, worldPositionStays: false);
        }
        
        public override void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            transform.position = worldPos;
        }
        
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            
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
                if (targetTray == ParentTray)
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
                    if (ParentTray != null)
                    {
                        ParentTray.RemoveItem(this);
                    }
                    
                    if (targetTray.AddItem(this, slotIndex))
                    {
                        ParentTray = targetTray;
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
            return DetectObjectUnderPointer<Tray>(eventData);
        }
        
        #endregion
    }
}
