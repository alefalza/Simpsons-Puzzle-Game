using Core;
using UnityEngine;
using UnityEngine.UI;
using Core.Managers;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverOverlay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    private SceneController sceneController;
    private string currentScene;

    private void Awake()
    {
        sceneController = ServiceLocator.Get<SceneController>();
        currentScene = SceneManager.GetActiveScene().name;

        retryButton.onClick.AddListener(OnRetryClicked);
        menuButton.onClick.AddListener(OnMenuClicked);
    }

    public void Show(int finalScore)
    {
        scoreText.text = $"Final Score: {finalScore}";
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        const float duration = 0.4f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void OnRetryClicked()
    {
        sceneController.LoadScene(currentScene);
    }

    private void OnMenuClicked()
    {
        sceneController.LoadScene("MainMenuScene");
    }

    private void OnDestroy()
    {
        retryButton.onClick.RemoveListener(OnRetryClicked);
        menuButton.onClick.RemoveListener(OnMenuClicked);
    }
}
