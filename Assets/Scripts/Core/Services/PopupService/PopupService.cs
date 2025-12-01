using System;
using UnityEngine;

namespace Core.Services.PopupService
{
    [Serializable]
    public class PopupService : IPopupService
    {
        private Transform popupRoot;

        public PopupService(Transform root)
        {
            popupRoot = root;
        }

        public void Initialize()
        {
            Debug.Log("[PopupService] Initializing...");
            
            if (popupRoot == null)
            {
                Debug.LogError("[PopupService] Missing popup root!");
            }
        }

        public void ShowPopup(string popupId)
        {
            if (popupRoot == null) return;
            Debug.Log($"[PopupService] Show popup: {popupId}");
            // Implement prefab/resource logic using popupRoot
        }

        public void HidePopup(string popupId)
        {
            Debug.Log($"[PopupService] Hide popup: {popupId}");
        }

        public void Shutdown()
        {
            Debug.Log("[PopupService] Shutting down...");
        }
    }
}
