using System.Collections;
using Core.Services.UIService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Services.SceneService
{
    public class SceneService : ISceneService
    {
        private readonly MonoBehaviour coroutineRunner;

        public SceneService(MonoBehaviour runner)
        {
            coroutineRunner = runner;
        }

        public void Initialize()
        {
            Debug.Log("[SceneService] Initializing...");
            
            if (coroutineRunner == null)
            {
                Debug.LogError("[SceneService] Missing coroutine runner!");
                return;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public void LoadScene(string sceneName)
        {
            if (coroutineRunner == null) return;
            
            coroutineRunner.StartCoroutine(LoadSceneRoutine(sceneName, LoadSceneMode.Single));
        }

        public void LoadSceneAdditive(string sceneName)
        {
            if (coroutineRunner == null) return;
            
            coroutineRunner.StartCoroutine(LoadSceneRoutine(sceneName, LoadSceneMode.Additive));
        }

        public void UnloadScene(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }

        private IEnumerator LoadSceneRoutine(string sceneName, LoadSceneMode mode)
        {
            // Show global overlay
            UIService.ShowLoadingOverlay(true);
            UIService.UpdateLoadingBar(0);

            // Optional: tiny delay to show overlay clearly
            yield return new WaitForSecondsRealtime(0.25f);

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);

            // Optionally show progress here
            while (!op.isDone)
            {
                // op.progress goes from 0 to 0.9.
                // We clamp/normalize it to 0-1 for the slider.
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                UIService.UpdateLoadingBar(progress);
                
                yield return null; // Wait for the next frame
            }

            // Hide global overlay
            UIService.ShowLoadingOverlay(false);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[SceneService] Scene loaded: {scene.name}");
        }

        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"[SceneService] Scene unloaded: {scene.name}");
        }

        public void Shutdown()
        {
            Debug.Log("[SceneService] Shutting down...");
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private IUIService uiService;
        private IUIService UIService => uiService ??= ServiceLocator.Get<IUIService>();
    }
}
