using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootLoader : MonoBehaviour
{
    [Header("Managers Root Prefab")]
    [SerializeField] private GameObject managersRootPrefab;

    [Header("Boot Overlay (local to BootScene)")]
    [SerializeField] private GameObject bootOverlay;

    private const string MAIN_MENU_SCENE = "MainMenuScene";

    private IEnumerator Start()
    {
        // Show boot overlay immediately
        if (bootOverlay != null)
            bootOverlay.SetActive(true);

        // Small delay so the UI actually appears on screen
        yield return new WaitForSeconds(0.2f);

        // -------------------------
        // 1) Instantiate ManagersRoot
        // -------------------------
        if (managersRootPrefab != null)
        {
            GameObject root = Instantiate(managersRootPrefab);
            root.transform.SetParent(null);
            DontDestroyOnLoad(root);
        }
        else
        {
            Debug.LogError("[BootLoader] ManagersRootPrefab is missing!");
        }

        // -------------------------
        // 2) Load Main Menu (Single)
        // -------------------------
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(MAIN_MENU_SCENE, LoadSceneMode.Single);

        // OPTIONAL: set main menu active on completion
        loadOp.completed += _ =>
        {
            Scene menu = SceneManager.GetSceneByName(MAIN_MENU_SCENE);
            SceneManager.SetActiveScene(menu);
        };

        yield return loadOp;

        // -------------------------
        // 3) Hide Boot Overlay
        // -------------------------
        if (bootOverlay != null)
            bootOverlay.SetActive(false);

        // -------------------------
        // 4) Destroy BootScene
        // -------------------------
        // At this point BootLoader exists in BootScene
        // LoadScene(Single) already unloaded BootScene,
        // but the GameObject survives unless we kill it manually.
        Destroy(gameObject);
    }
}
