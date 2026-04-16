using System.Collections.Generic;
using GameModes.DrinkSort.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModes.DrinkSort.Gameplay
{
    [CreateAssetMenu(menuName = "GameModes/DrinkSort/ItemData")]
    public class DrinkSortItemData : ScriptableObject
    {
        [System.Serializable]
        public class ItemData
        {
            public SortableItemType itemType;
            public Sprite sprite;
        }
        
        [SerializeField] private ItemData[] availableItems;
        
        private readonly Dictionary<SortableItemType, Sprite> sprites = new Dictionary<SortableItemType, Sprite>();

        private void OnEnable()
        {
            InitializeSprites();
        }
        
        private void InitializeSprites()
        {
            sprites.Clear();

            if (availableItems != null)
            {
                foreach (var item in availableItems)
                {
                    sprites[item.itemType] = item.sprite;
                }
            }
        }

        public Sprite GetSpriteForType(SortableItemType itemType)
        {
            return sprites.GetValueOrDefault(itemType);
        }
        
        public SortableItemType GetRandomAvailableType()
        {
            if (availableItems == null || availableItems.Length == 0)
            {
                return SortableItemType.None;
            }
            
            return availableItems[Random.Range(0, availableItems.Length)].itemType;
        }

        public List<SortableItemType> GetAvailableTypes()
        {
            List<SortableItemType> availableTypes = new List<SortableItemType>();
            if (availableItems == null)
            {
                return availableTypes;
            }

            foreach (var itemData in availableItems)
            {
                if (itemData == null || itemData.itemType == SortableItemType.None || availableTypes.Contains(itemData.itemType))
                {
                    continue;
                }

                availableTypes.Add(itemData.itemType);
            }

            return availableTypes;
        }
    }
}
