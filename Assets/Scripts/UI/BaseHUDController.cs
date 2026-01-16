using Core;
using Core.Services.PopupService;
using UI.Popups;
using UnityEngine;

namespace UI
{
    public abstract class BaseHUDController : MonoBehaviour, IHUDController
    {
        [Header("UI Elements")]
        [SerializeField] private ScoreUI scoreUI;

        [Header("Popup Definitions")]
        [SerializeField] private PopupDefinition pausePopupDefinition;
        [SerializeField] private PopupDefinition gameOverPopupDefinition;

        private IPopupService popupService;

        private BasePopup OpenedPopup => popupService.GetOpenedPopup();

        private void Awake()
        {
            popupService = ServiceLocator.Get<IPopupService>();
        }

        public virtual void UpdateScore(int newScore)
        {
            scoreUI.SetScore(newScore);
        }

        public virtual bool CanTogglePause()
        {
            return OpenedPopup is null || OpenedPopup is PausePopup && !OpenedPopup.IsFading;
        }

        public virtual void ShowPausePopup()
        {   
            popupService.Push(pausePopupDefinition, new PausePopupData(Priority.Low, OnResumeClicked));
        }

        protected abstract void OnResumeClicked();

        public virtual void HidePausePopup()
        {
            OpenedPopup.Close();
        }

        public virtual void ShowGameOverOverlay(int finalScore)
        {
            popupService.Push(gameOverPopupDefinition, new GameOverPopupData(Priority.Low, finalScore));
        }
    }
}
