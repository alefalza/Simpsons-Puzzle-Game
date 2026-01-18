using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.PopupService
{
    public class PopupService : IPopupService
    {
        private readonly IPopupFactory factory;
        private readonly Transform container;
        private readonly Dictionary<string, IPopUp> popups = new();

        private IPopUp openedPopup;
        
        public PopupService(IPopupFactory factory, Transform container)
        {
            this.factory = factory;
            this.container = container;
        }

        public void Initialize()
        {
            Debug.Log("[PopupService] Initializing...");
        }
    
        public void Push(PopupDefinition def, PopupData data)
        {
            openedPopup = GetOrCreatePopup(def);

            openedPopup.OnClosed -= OnPopupClosed;
            openedPopup.OnClosed += OnPopupClosed;
            
            openedPopup.Setup(data, def);
            openedPopup.Open();
        }

        private IPopUp GetOrCreatePopup(PopupDefinition definition)
        {
            if (popups.TryGetValue(definition.id, out var existing))
            {
                if (!existing.IsActive)
                    existing.SetActive(true);

                return existing;
            }

            var newPopup = factory.CreatePopup(definition, container);
            popups.Add(definition.id, newPopup);
            
            return newPopup;
        }

        public IPopUp GetOpenedPopup() => openedPopup;
        
        private void OnPopupClosed(bool destroyOnClose)
        {
            if (openedPopup == null) return;

            var id = openedPopup.Definition.id;

            openedPopup.OnClosed -= OnPopupClosed;

            if (destroyOnClose)
                popups.Remove(id);

            openedPopup = null;
        }
        
        public void Shutdown()
        {
            Debug.Log("[PopupService] Shutting down...");
        }
    }
}
