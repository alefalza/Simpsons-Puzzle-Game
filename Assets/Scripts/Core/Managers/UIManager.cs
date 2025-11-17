using UnityEngine;

namespace Core.Managers
{
    public class UIManager : MonoBehaviour, IService
    {
        [Header("Global UI Elements")]
        [SerializeField] private GameObject loadingOverlay;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        /// <summary>
        /// Shows or hides the global loading overlay.
        /// </summary>
        public void ShowLoading(bool show)
        {
            if (loadingOverlay != null)
                loadingOverlay.SetActive(show);
        }
    }
}
