using UI.MainMenu;
using UnityEngine;

namespace Core.Services.UIService
{
    public class UIService : IUIService
    {
        private readonly GameObject loadingOverlay;

        public MainMenuTab LastTabSeen { get; private set; } = MainMenuTab.Home;
        
        public UIService(GameObject overlay)
        {
            loadingOverlay = overlay;
        }
        
        public void Initialize()
        {
            Debug.Log("[UIService] Initializing...");
            
            if (loadingOverlay == null)
            {
                Debug.LogError("[UIService] Missing loading overlay!");
            }
        }

        /// <summary>
        /// Shows or hides the global loading overlay.
        /// </summary>
        public void ShowLoadingOverlay(bool show)
        {
            if (loadingOverlay != null)
                loadingOverlay.SetActive(show);
        }

        public void SetLastTabSeen(MainMenuTab tab)
        {
            LastTabSeen = tab;
        }

        public void Shutdown()
        {
            Debug.Log("[UIService] Shutting down...");
        }
    }
}
