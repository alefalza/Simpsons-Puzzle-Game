using Core;
using UnityEngine;
using UnityEngine.UI;
using Core.Managers;

public class GameHUDOverlay : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private SceneController sceneController;

    private void Awake()
    {
        sceneController = ServiceLocator.Get<SceneController>();
        backButton.onClick.AddListener(ReturnToMenu);
    }

    private void ReturnToMenu()
    {
        sceneController.LoadScene("MainMenuScene");
    }
}
