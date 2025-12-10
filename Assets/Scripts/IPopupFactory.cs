using UnityEngine;

public interface IPopupFactory
{
    BasePopup CreatePopup(PopupDefinition definition, Transform parent);
}
