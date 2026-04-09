using Core;
using Core.Services.PopupService;
using TMPro;
using UI.Popups;
using UnityEngine;

namespace UI
{
    public abstract class BaseHUDController : MonoBehaviour, IHUDController
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private ScoreUI scoreUI;

        [Header("Popup Definitions")]
        [SerializeField] private PopupDefinition pausePopupDefinition;
        [SerializeField] private PopupDefinition gameOverPopupDefinition;
        [SerializeField] private PopupDefinition winPopupDefinition;

        private IPopUp OpenedPopup => PopupService.GetOpenedPopup();

        public void SetLevelText(int levelNumber)
        {
            levelText.text = $"Level: {levelNumber.ToString()}";
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
            PopupService.Push(pausePopupDefinition, new PausePopupData(pausePopupDefinition.defaultPriority, OnResumeClicked));
        }

        protected abstract void OnResumeClicked();

        public virtual void HidePausePopup()
        {
            if (OpenedPopup != null && OpenedPopup is PausePopup && !OpenedPopup.IsFading)
            {
                OpenedPopup.Close();
            }
        }

        public virtual void ShowGameOverPopup(int finalScore)
        {
            PopupService.Push(gameOverPopupDefinition, new GameOverPopupData(gameOverPopupDefinition.defaultPriority, finalScore));
        }

        public virtual void ShowWinPopup(int finalScore)
        {
            PopupService.Push(winPopupDefinition, new WinPopup.WinPopupData(winPopupDefinition.defaultPriority, finalScore));
        }
        
        private IPopupService popupService;
        private IPopupService PopupService => popupService ??= ServiceLocator.Get<IPopupService>();
    }
}
