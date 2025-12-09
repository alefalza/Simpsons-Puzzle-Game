using Core;
using UnityEngine;

namespace Boot
{
    /// <summary>
    /// Bootstrap that initializes all services at startup
    /// Holds references to scene components that services might need
    /// </summary>
    public class ServiceBootstrap : MonoBehaviour
    {
        [Header("Definitions")]
        [SerializeField] private ServiceConfiguration serviceDefinitions;
        [SerializeField] private PopupLibrary popupLibrary;

        [Header("Audio Components")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("UI Components")]
        [SerializeField] private Transform popupRoot;
        [SerializeField] private GameObject loadingOverlay;

        public AudioSource MusicSource => musicSource;
        public AudioSource SfxSource => sfxSource;
        public Transform PopupRoot => popupRoot;
        public GameObject LoadingOverlay => loadingOverlay;
        public PopupLibrary PopupLibrary => popupLibrary;

        private void Awake()
        {
            if (serviceDefinitions == null)
            {
                Debug.LogError("[ServiceBootstrap] No ServiceConfiguration assigned!");
                return;
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.Initialize(serviceDefinitions, this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Shutdown();
        }
    }
}
