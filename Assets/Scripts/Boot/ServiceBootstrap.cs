using Core;
using UnityEngine;
using UnityEngine.Audio;

namespace Boot
{
    /// <summary>
    /// Bootstrap that initializes all services at startup.
    /// Holds references to scene components that services might need.
    /// </summary>
    public class ServiceBootstrap : MonoBehaviour
    {
        [Header("Definitions")]
        [SerializeField] private ServiceConfiguration serviceDefinitions;

        [Header("Audio Components")]
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("UI Components")]
        [SerializeField] private Transform popupRoot;
        [SerializeField] private GameObject loadingOverlay;

        public AudioMixer MainMixer => mainMixer;
        public AudioSource MusicSource => musicSource;
        public AudioSource SfxSource => sfxSource;
        public Transform PopupRoot => popupRoot;
        public GameObject LoadingOverlay => loadingOverlay;

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
