using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Boot
{
    public class BootLoader : MonoBehaviour
    {
        [Header("Managers Root Prefab")]
        [SerializeField] private ServiceBootstrap managersRoot;

        [Header("Boot Overlay (local to BootScene)")]
        [SerializeField] private GameObject bootOverlay;
        [SerializeField] private Slider loadingBar;

        private IEnumerator Start()
        {
            if (bootOverlay != null)
                bootOverlay.SetActive(true);

            // Small delay so the UI actually appears on screen
            yield return new WaitForSeconds(0.25f);

            if (managersRoot != null)
            {
                ServiceBootstrap root = Instantiate(managersRoot, null);
                //DontDestroyOnLoad(root);
            }
            else
            {
                Debug.LogError("[BootLoader] ManagersRootPrefab is missing!");
            }

            // Load Main Menu (Single)
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(GameConstants.MAIN_MENU_SCENE, LoadSceneMode.Single);

            while (!loadOp.isDone)
            {
                // loadOp.progress goes from 0 to 0.9.
                // We clamp/normalize it to 0-1 for the slider.
                float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
                loadingBar.value = progress;
                
                yield return null; // Wait for the next frame
            }
            
            // Set main menu active on completion
            loadOp.completed += _ =>
            {
                Scene menu = SceneManager.GetSceneByName(GameConstants.MAIN_MENU_SCENE);
                SceneManager.SetActiveScene(menu);
            };

            yield return loadOp;

            if (bootOverlay != null)
                bootOverlay.SetActive(false);

            Destroy(gameObject);
        }
    }
}
