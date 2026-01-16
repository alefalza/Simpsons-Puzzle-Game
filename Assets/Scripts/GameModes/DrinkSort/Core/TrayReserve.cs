using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModes.DrinkSort.Core
{
    [System.Serializable]
    public class TrayReserve
    {
        [System.Serializable]
        public class ItemData
        {
            public SortableItemType itemType;
            public int weight = 100;
        }
        
        [SerializeField] private ItemData[] availableItems;
        private Queue<SortableItemType> reserveQueue = new Queue<SortableItemType>();
        
        public int ReserveCount => reserveQueue.Count;
        
        public void Initialize(ItemData[] items, int initialSize)
        {
            availableItems = items;
            reserveQueue.Clear();
            
            for (int i = 0; i < initialSize; i++)
            {
                SortableItemType randomType = GetRandomItemType();
                reserveQueue.Enqueue(randomType);
            }
        }
        
        public SortableItemType PopNextItem()
        {
            if (reserveQueue.Count == 0)
            {
                return SortableItemType.None;
            }
            
            return reserveQueue.Dequeue();
        }
        
        public void AddItemsToReserve(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SortableItemType randomType = GetRandomItemType();
                reserveQueue.Enqueue(randomType);
            }
        }
        
        private SortableItemType GetRandomItemType()
        {
            if (availableItems == null || availableItems.Length == 0)
            {
                return SortableItemType.None;
            }
            
            // Weighted random selection
            int totalWeight = 0;
            foreach (var item in availableItems)
            {
                totalWeight += item.weight;
            }
            
            if (totalWeight <= 0)
            {
                return availableItems[0].itemType;
            }
            
            int randomValue = Random.Range(0, totalWeight);
            int currentWeight = 0;
            
            foreach (var item in availableItems)
            {
                currentWeight += item.weight;
                if (randomValue < currentWeight)
                {
                    return item.itemType;
                }
            }
            
            return availableItems[availableItems.Length - 1].itemType;
        }
        
        public bool HasItems()
        {
            return reserveQueue.Count > 0;
        }
    }
}



