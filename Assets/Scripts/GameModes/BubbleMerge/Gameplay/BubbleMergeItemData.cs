using System.Collections.Generic;
using GameModes.BubbleMerge.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "GameModes/BubbleMerge/ItemData")]
public class BubbleMergeItemData : ScriptableObject
{
    [System.Serializable]
    public class ItemData
    {
        public int tier;
        public Bubble prefab;
    }
    
    [SerializeField] private ItemData[] availableItems;

    private readonly Dictionary<int, Bubble> sprites = new Dictionary<int, Bubble>();
    
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
                sprites[item.tier] = item.prefab;
            }
        }
    }
    
    public Bubble GetPrefabForColor(int tier)
    {
        return sprites.GetValueOrDefault(tier);
    }
        
    public int GetRandomAvailableTier()
    {
        if (availableItems == null || availableItems.Length == 0)
        {
            return -1;
        }
            
        return availableItems[Random.Range(0, availableItems.Length)].tier;
    }
    
    public List<int> GetAvailableTiers()
    {
        List<int> availableTiers = new List<int>();
        if (availableItems == null)
        {
            return availableTiers;
        }

        foreach (var itemData in availableItems)
        {
            if (itemData == null || itemData.tier == -1 || availableTiers.Contains(itemData.tier))
            {
                continue;
            }

            availableTiers.Add(itemData.tier);
        }

        return availableTiers;
    }
}
