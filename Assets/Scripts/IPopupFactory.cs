using Core.Services.PopupService;
using UnityEngine;

namespace Core.Services.PopupService
{
    public interface IPopupFactory
    {
        BasePopup CreatePopup(PopupDefinition definition, Transform parent);
    }
}
