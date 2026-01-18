using UnityEngine;

public class PopupFactory : IPopupFactory
{
    public IPopUp CreatePopup(PopupDefinition definition, Transform parent)
    {
        return Object.Instantiate(definition.prefab, parent);
    }
}
