using System.Collections.Generic;
using GameModes.DrinkSort.Gameplay;
using UnityEngine;

namespace GameModes.DrinkSort.Core
{
    public class Tray : MonoBehaviour
    {
        [SerializeField] private int maxItems = 3;
        [SerializeField] private Transform itemsContainer;
        [SerializeField] private Vector2Int gridPosition;
        
        private TrayReserve reserve;
        private readonly List<SortableItem> items = new List<SortableItem>();
        private int trayIndex;
        
        public int ItemCount => items.Count;
        public bool IsFull => items.Count >= maxItems;
        public bool IsEmpty => items.Count == 0;
        public Vector2Int GridPosition => gridPosition;
        public int MaxItems => maxItems;
        public TrayReserve Reserve => reserve;
        public int ReserveCount => reserve.ReserveCount;
        
        private void Awake()
        {
            if (itemsContainer == null)
            {
                itemsContainer = transform;
            }
            
            reserve = new TrayReserve();
        }
        
        public void Initialize(Vector2Int position, int index)
        {
            gridPosition = position;
            trayIndex = index;
            gameObject.name = $"Tray_{position.x}_{position.y}";
        }
        
        public void InitializeReserve(TrayReserve.ItemData[] availableItems, int initialReserveSize)
        {
            if (reserve == null)
            {
                reserve = new TrayReserve();
            }
            reserve.Initialize(availableItems, initialReserveSize);
        }
        
        public bool CanAddItem()
        {
            return items.Count < maxItems;
        }
        
        public bool AddItem(SortableItem item)
        {
            if (items.Count >= maxItems || item == null)
            {
                return false;
            }
            
            items.Add(item);
            item.SetTray(this);
            item.transform.SetParent(itemsContainer);
            
            ArrangeItems();
            
            // Notify GameManager to check for matches
            if (items.Count >= 3)
            {
                GameModes.DrinkSort.Gameplay.DrinkSortGameManager.Instance?.CheckTrayForMatch(this);
            }
            
            return true;
        }
        
        public bool RemoveItem(SortableItem item)
        {
            if (item == null || !items.Contains(item))
            {
                return false;
            }
            
            items.Remove(item);
            item.SetTray(null);
            ArrangeItems();
            
            return true;
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
        }
        
        public void PopulateFromReserve(int itemCount, SortableItem itemPrefab, System.Func<SortableItemType, Sprite> getSpriteFunc, Transform itemsParent)
        {
            for (int i = 0; i < itemCount && CanAddItem() && reserve.HasItems(); i++)
            {
                SortableItemType itemType = reserve.PopNextItem();
                if (itemType == SortableItemType.None) continue;
                
                SortableItem newItem = Instantiate(itemPrefab, itemsParent != null ? itemsParent : transform);
                Sprite itemSprite = getSpriteFunc != null ? getSpriteFunc(itemType) : null;
                
                newItem.Initialize(itemType, itemSprite);
                AddItem(newItem);
            }
        }
        
        private void ArrangeItems()
        {
            if (itemsContainer == null) return;
            
            // Organizar items en una fila horizontal
            float spacing = 0.5f;
            float startX = -(items.Count - 1) * spacing * 0.5f;
            
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    Vector3 localPos = new Vector3(startX + i * spacing, 0, 0);
                    items[i].transform.localPosition = localPos;
                }
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

