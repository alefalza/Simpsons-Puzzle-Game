using System.Collections.Generic;
using GameModes.DonutStack.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "GameModes/DonutStack/ItemData")]
public class DonutStackItemData : ScriptableObject
{
    [System.Serializable]
    public class ItemData
    {
        public DonutColor color;
        public Sprite sprite;
    }
    
    [SerializeField] private ItemData[] availableItems;
    
    private readonly Dictionary<DonutColor, Sprite> sprites = new Dictionary<DonutColor, Sprite>();

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
                sprites[item.color] = item.sprite;
            }
        }
    }
    
    public Sprite GetSpriteForColor(DonutColor color)
    {
        return sprites.GetValueOrDefault(color);
    }
        
    public DonutColor GetRandomAvailableColor()
    {
        if (availableItems == null || availableItems.Length == 0)
        {
            return DonutColor.None;
        }
            
        return availableItems[Random.Range(0, availableItems.Length)].color;
    }
    
    public List<DonutColor> GetAvailableColors()
    {
        List<DonutColor> availableTypes = new List<DonutColor>();
        if (availableItems == null)
        {
            return availableTypes;
        }

        foreach (var itemData in availableItems)
        {
            if (itemData == null || itemData.color == DonutColor.None || availableTypes.Contains(itemData.color))
            {
                continue;
            }

            availableTypes.Add(itemData.color);
        }

        return availableTypes;
    }
}
