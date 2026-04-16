using System.Collections.Generic;
using GameModes.DrinkSort.Gameplay;
using UnityEngine;

namespace GameModes.DrinkSort.Core
{
    public class Tray : MonoBehaviour
    {
        private int maxItems = 3;
        [SerializeField] private Transform[] slots = new Transform[3];

        private Vector2Int gridPosition;
        private readonly List<SortableItem> items = new List<SortableItem>();
        private SortableItem[] slotItems = new SortableItem[3]; // Items in each slot
        private int trayIndex;
        
        public int ItemCount => items.Count;
        public bool IsFull => items.Count >= maxItems;
        public bool IsEmpty => items.Count == 0;
        public Vector2Int GridPosition => gridPosition;
        public int MaxItems => maxItems;
        
        private void Awake()
        {
            // Validate that there are 3 slots
            if (slots == null || slots.Length != maxItems)
            {
                Debug.LogError($"[Tray] Se requieren exactamente {maxItems} slots!");
            }
        }
        
        public void Initialize(Vector2Int position, int index)
        {
            gridPosition = position;
            trayIndex = index;
            gameObject.name = $"Tray_{position.x}_{position.y}";
        }
        
        public bool CanAddItem()
        {
            return items.Count < maxItems && GetFreeSlotIndex() != -1;
        }
        
        public bool AddItem(SortableItem item, int slotIndex = -1)
        {
            if (items.Count >= maxItems || item == null)
            {
                return false;
            }
            
            // If no slot is specified, look for a free one
            if (slotIndex == -1)
            {
                slotIndex = GetFreeSlotIndex();
            }
            
            // Validate slot
            if (slotIndex < 0 || slotIndex >= maxItems || slotItems[slotIndex] != null)
            {
                return false;
            }
            
            return AddItemToSlot(item, slotIndex);
        }
        
        private bool AddItemToSlot(SortableItem item, int slotIndex, bool notifyMatchCheck = true)
        {
            if (slotIndex < 0 || slotIndex >= maxItems || slotItems[slotIndex] != null)
            {
                return false;
            }
            
            items.Add(item);
            slotItems[slotIndex] = item;
            item.SetTray(this);
            
            // Position in the slot
            Transform slotTransform = slots[slotIndex] != null ? slots[slotIndex] : transform;
            item.transform.SetParent(slotTransform);
            item.transform.localPosition = Vector3.zero;
            
            // Notify GameManager to check for matches
            if (notifyMatchCheck && items.Count >= 3)
            {
                DrinkSortGameManager.Instance?.CheckTrayForMatch(this);
            }
            
            return true;
        }

        public bool TrySpawnInitialItem(SortableItem itemPrefab, SortableItemType itemType, System.Func<SortableItemType, Sprite> getSpriteFunc)
        {
            if (itemPrefab == null || getSpriteFunc == null || itemType == SortableItemType.None || !CanAddItem())
            {
                return false;
            }

            int freeSlotIndex = GetFreeSlotIndex();
            if (freeSlotIndex == -1)
            {
                return false;
            }

            Transform slotTransform = slots[freeSlotIndex] != null ? slots[freeSlotIndex] : transform;
            SortableItem newItem = Instantiate(itemPrefab, slotTransform);
            Sprite itemSprite = getSpriteFunc(itemType);
            newItem.Initialize(itemType, itemSprite);

            return AddItemToSlot(newItem, freeSlotIndex, notifyMatchCheck: false);
        }
        
        public bool RemoveItem(SortableItem item)
        {
            if (item == null || !items.Contains(item))
            {
                return false;
            }
            
            // Find and free the slot
            for (int i = 0; i < slotItems.Length; i++)
            {
                if (slotItems[i] == item)
                {
                    slotItems[i] = null;
                    break;
                }
            }
            
            items.Remove(item);
            item.SetTray(null);
            
            return true;
        }
        
        private int GetFreeSlotIndex()
        {
            for (int i = 0; i < maxItems; i++)
            {
                if (slotItems[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
        
        /// <summary>
        /// Finds the closest slot to the position.
        /// </summary>
        public int GetSlotIndexForPosition(Vector3 worldPosition)
        {
            float minDistance = float.MaxValue;
            int closestSlot = -1;
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null) continue;
                
                float distance = Vector3.Distance(worldPosition, slots[i].position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestSlot = i;
                }
            }
            
            return closestSlot;
        }
        
        public bool IsSlotFree(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxItems)
            {
                return false;
            }
            
            return slotItems[slotIndex] == null;
        }
        
        public List<SortableItem> GetItems()
        {
            return new List<SortableItem>(items);
        }
        
        public void ClearItems()
        {
            foreach (var item in items)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            
            items.Clear();
            
            // Clear slots
            for (int i = 0; i < slotItems.Length; i++)
            {
                slotItems[i] = null;
            }
        }
        
        public bool HasMatch()
        {
            if (items.Count != 3) return false;
            
            SortableItemType type = items[0].ItemType;
            return items[1].ItemType == type && items[2].ItemType == type;
        }
        
        public SortableItemType GetMatchType()
        {
            if (!HasMatch()) return SortableItemType.None;
            return items[0].ItemType;
        }
    }
}

