using UnityEngine;

namespace Core.Managers
{
    public class PopupManager : MonoBehaviour, IService
    {
        [Header("Popup Root")]
        [SerializeField] private Transform popupRoot;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        /// <summary>
        /// Instantiates a popup prefab inside the popup root.
        /// </summary>
        public GameObject ShowPopup(GameObject popupPrefab)
        {
            return Instantiate(popupPrefab, popupRoot);
        }
    }
}
