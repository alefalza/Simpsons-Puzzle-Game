using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Services.PopupService
{
    public class PopupService : IPopupService
    {
        private readonly IPopupFactory factory;
        private readonly Transform container;

        private readonly List<(PopupDefinition def, PopupData data)> queue = new();

        private BasePopup openedPopup;
        private (PopupDefinition def, PopupData data)? hiddenPopup;

        public PopupService(IPopupFactory factory, Transform container)
        {
            this.factory = factory;
            this.container = container;
        }

        public void Initialize()
        {
            
        }
    
        public void Push(PopupDefinition def, PopupData data)
        {
            queue.Add((def, data));
            TryShowNext();
        }
        
        public BasePopup GetOpenedPopup() => openedPopup;
        
        private void TryShowNext()
        {
            // case 1: reopening hidden popup
            if (openedPopup == null && hiddenPopup.HasValue)
            {
                var (def, data) = hiddenPopup.Value;
                hiddenPopup = null;

                openedPopup = factory.CreatePopup(def, data, container);
                openedPopup.OnClosed += OnPopupClosed;
                openedPopup.Open();
                return;
            }

            // nothing to open
            if (openedPopup != null)
            {
                var next = queue.OrderByDescending(q => q.data.Priority).FirstOrDefault();

                // if next is not urgent, do nothing
                if (!next.Equals(default((PopupDefinition, PopupData))) &&
                    next.data.Priority == Priority.Urgent)
                {
                    // urgent overrides current
                    openedPopup.Close();
                    hiddenPopup = (openedPopup.Definition, openedPopup.PopupData);
                    openedPopup = null;

                    queue.Remove(next);
                    openedPopup = factory.CreatePopup(next.def, next.data, container);
                    openedPopup.OnClosed += OnPopupClosed;
                    openedPopup.Open();
                }

                return;
            }

            // case 3: no popup active, open next
            if (queue.Count > 0)
            {
                var next = queue.OrderByDescending(q => q.data.Priority).First();
                queue.Remove(next);

                openedPopup = factory.CreatePopup(next.def, next.data, container);
                openedPopup.OnClosed += OnPopupClosed;
                openedPopup.Open();
            }
        }

        private void OnPopupClosed()
        {
            openedPopup.OnClosed -= OnPopupClosed;
            openedPopup = null;
            TryShowNext();
        }
        
        public void Shutdown()
        {
            
        }
    }
}
