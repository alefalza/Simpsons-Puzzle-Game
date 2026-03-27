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
            public Color itemColor = Color.white;
        }
        
        [SerializeField] private ItemData[] availableItems;
        [SerializeField] private SortableItem itemPrefab;
        
        private Dictionary<SortableItemType, Color> itemColors = new Dictionary<SortableItemType, Color>();
        
        public SortableItem ItemPrefab => itemPrefab;
        
        private void OnEnable()
        {
            InitializeColors();
        }
        
        private void InitializeColors()
        {
            itemColors.Clear();
            
            if (availableItems != null)
            {
                foreach (var itemData in availableItems)
                {
                    itemColors[itemData.itemType] = itemData.itemColor;
                }
            }
        }
        
        public Color GetColorForType(SortableItemType itemType)
        {
            if (itemColors.TryGetValue(itemType, out Color color))
            {
                return color;
            }
            
            return Color.white;
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
