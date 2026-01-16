using Core.Services.PopupService;
using UnityEngine;

namespace Core.Services.PopupService
{
    public class PopupFactory : IPopupFactory
    {
        public BasePopup CreatePopup(PopupDefinition definition, Transform parent)
        {
            return Object.Instantiate(definition.prefab, parent);
        }
    }
}
