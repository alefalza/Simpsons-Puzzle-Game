using System.Collections.Generic;
using GameModes.DrinkSort.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModes.DrinkSort.Gameplay
{
    [CreateAssetMenu(menuName = "GameModes/DrinkSort/ItemReserve")]
    public class ItemReserve : ScriptableObject
    {
        [System.Serializable]
        public class ItemData
        {
            public SortableItemType itemType;
            public Sprite itemSprite;
            public int weight = 100;
        }
        
        [SerializeField] private ItemData[] availableItems;
        [SerializeField] private SortableItem itemPrefab;
        
        private Queue<SortableItemType> reserveQueue = new Queue<SortableItemType>();
        private Dictionary<SortableItemType, Sprite> itemSprites = new Dictionary<SortableItemType, Sprite>();
        
        public int ReserveCount => reserveQueue.Count;
        public SortableItem ItemPrefab => itemPrefab;
        
        private void OnEnable()
        {
            InitializeSprites();
        }
        
        private void InitializeSprites()
        {
            itemSprites.Clear();
            
            if (availableItems != null)
            {
                foreach (var itemData in availableItems)
                {
                    if (itemData.itemSprite != null)
                    {
                        itemSprites[itemData.itemType] = itemData.itemSprite;
                    }
                }
            }
        }
        
        public void Initialize(int initialReserveSize)
        {
            reserveQueue.Clear();
            
            for (int i = 0; i < initialReserveSize; i++)
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
        
        public Sprite GetSpriteForType(SortableItemType itemType)
        {
            itemSprites.TryGetValue(itemType, out Sprite sprite);
            return sprite;
        }
        
        public bool HasItems()
        {
            return reserveQueue.Count > 0;
        }
    }
}



