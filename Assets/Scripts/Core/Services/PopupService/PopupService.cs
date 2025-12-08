using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.PopupService
{
    [Serializable]
    public class PopupService : IPopupService
    {
        private Transform popupContainer;

        private readonly Dictionary<string, BasePopup> activePopups = new();

        public PopupService(Transform container)
        {
            popupContainer = container;
        }

        public void Initialize()
        {
            Debug.Log("[PopupService] Initializing...");
            
            if (popupContainer == null)
            {
                Debug.LogError("[PopupService] Missing popup root!");
            }
        }
        
        public T Show<T>(PopupDefinition definition) where T : BasePopup
        {
            if (activePopups.TryGetValue(definition.popupId, out var popup))
                return popup as T;

            var instance = UnityEngine.Object.Instantiate(definition.prefab, popupContainer);
            instance.definition = definition;
            instance.name = definition.popupId;

            activePopups[definition.popupId] = instance;

            instance.Open();

            return instance as T;
        }

        public void Close(string popupId)
        {
            if (!activePopups.TryGetValue(popupId, out var popup))
                return;

            popup.Close();
            activePopups.Remove(popupId);
        }

        public void Close(BasePopup popup)
        {
            if (popup == null) return;
            
            activePopups.Remove(popup.definition.popupId);
            popup.Close();
        }
        
        public void Shutdown()
        {
            Debug.Log("[PopupService] Shutting down...");
        }
    }
}
