using System.Collections;
using Core;
using Core.Services.SceneService;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Overlays
{
    public class GameOverOverlay : BasePopup
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;

        private SceneService sceneService;
        private string currentScene;

        protected override void Awake()
        {
            base.Awake();
            
            sceneService = ServiceLocator.Get<SceneService>();
            currentScene = SceneManager.GetActiveScene().name;

            retryButton.onClick.AddListener(OnRetryClicked);
            menuButton.onClick.AddListener(OnBackToMenuClicked);
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
            sceneService.LoadScene(currentScene);
        }

        private void OnBackToMenuClicked()
        {
            sceneService.LoadScene("MainMenuScene");
            Destroy(gameObject); // So it does not overlap with the Loading screen
        }

        private void OnDestroy()
        {
            retryButton.onClick.RemoveListener(OnRetryClicked);
            menuButton.onClick.RemoveListener(OnBackToMenuClicked);
        }
    }
}
