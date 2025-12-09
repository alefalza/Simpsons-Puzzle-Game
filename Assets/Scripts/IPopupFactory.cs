using UnityEngine;

public interface IPopupFactory
{
    BasePopup CreatePopup(PopupDefinition def, PopupData data, Transform parent);
}
