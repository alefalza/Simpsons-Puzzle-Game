using System;
using System.Collections;
using Core;
using Core.Managers;
using GameModes.BubbleMerge.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Overlays
{
    public class PauseOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button menuButton;

        private SceneController sceneController;

        public bool IsFading { get; private set; } = false;
    
        private void Awake()
        {
            sceneController = ServiceLocator.Get<SceneController>();

            resumeButton.onClick.AddListener(OnResumePressed);
            menuButton.onClick.AddListener(OnMenuPressed);
        }

        public void Show()
        {
            StartCoroutine(FadeIn());
        }

        public void Hide()
        {
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeIn()
        {
            IsFading = true;

            canvasGroup.alpha = 0;
        
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime * 4f;
                yield return null;
            }
        
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        
            IsFading = false;
        }

        private IEnumerator FadeOut()
        {
            IsFading = true;
        
            canvasGroup.alpha = 1;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime * 4f;
                yield return null;
            }

            IsFading = false;
        }

        private void OnResumePressed()
        {
            BubbleGameManager.Instance.TogglePauseFromOverlay();
        }

        private void OnMenuPressed()
        {
            sceneController.LoadScene("MainMenuScene");
        }

        private void OnDestroy()
        {
            resumeButton.onClick.RemoveListener(OnResumePressed);
            menuButton.onClick.RemoveListener(OnMenuPressed);
        }
    }
}
