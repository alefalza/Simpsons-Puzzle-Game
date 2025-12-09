using UnityEngine;

public class PopupFactory : IPopupFactory
{
    public BasePopup CreatePopup(PopupDefinition def, PopupData data, Transform parent)
    {
        var instance = Object.Instantiate(def.prefab, parent);
        instance.Setup(data, def);
        
        return instance;
    }
}
