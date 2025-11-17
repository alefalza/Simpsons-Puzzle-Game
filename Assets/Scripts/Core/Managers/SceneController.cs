using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Managers
{
    public class SceneController : MonoBehaviour, IService
    {
        [SerializeField] private GameObject gameHUDOverlayPrefab;
        
        private UIManager uiManager;

        private void Awake()
        {
            // Re-register events every time the object wakes
            SceneManager.sceneLoaded -= OnUnitySceneLoaded;
            SceneManager.sceneLoaded += OnUnitySceneLoaded;

            SceneManager.sceneUnloaded -= OnUnitySceneUnloaded;
            SceneManager.sceneUnloaded += OnUnitySceneUnloaded;

            ServiceLocator.Register(this);
        }

        private void Start()
        {
            uiManager = ServiceLocator.Get<UIManager>();
        }

        #region Public API
        /// <summary>
        /// Loads a scene in SINGLE mode.
        /// Shows loading overlay automatically.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, LoadSceneMode.Single));
        }

        /// <summary>
        /// Loads a scene ADDITIVELY.
        /// </summary>
        public void LoadSceneAdditive(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, LoadSceneMode.Additive));
        }

        /// <summary>
        /// Unloads an additive scene.
        /// </summary>
        public void UnloadScene(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        #endregion
        
        private IEnumerator LoadSceneRoutine(string sceneName, LoadSceneMode mode)
        {
            // Show global overlay
            uiManager.ShowLoading(true);

            // Optional: tiny delay to show overlay clearly
            yield return new WaitForSeconds(0.25f);

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);

            // Optionally show progress here
            while (!op.isDone)
                yield return null;

            // Hide global overlay
            uiManager.ShowLoading(false);
        }

        #region Unity Scene Events
        private void OnUnitySceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string loadType = mode == LoadSceneMode.Single ? "SINGLE" : "ADDITIVE";
            Debug.Log($"[SceneController] Scene LOADED → {scene.name} ({loadType})");
            
            // If we're in a GameMode scene (anything except MainMenuScene)
            // instantiate the HUD overlay
            if (mode == LoadSceneMode.Single && scene.name != "MainMenuScene")
            {
                if (gameHUDOverlayPrefab != null)
                {
                    Instantiate(gameHUDOverlayPrefab);
                }
            }
        }

        private void OnUnitySceneUnloaded(Scene scene)
        {
            Debug.Log($"[SceneController] Scene UNLOADED → {scene.name}");
        }
        #endregion

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnUnitySceneLoaded;
            SceneManager.sceneUnloaded -= OnUnitySceneUnloaded;
        }
    }
}
