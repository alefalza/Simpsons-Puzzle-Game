using Core.Services.PopupService;
using UnityEngine;

public interface IPopupFactory
{
    IPopUp CreatePopup(PopupDefinition definition, Transform parent);
}
