using System;
using Core;
using Core.Services.PopupService;
using Core.Services.SceneService;
using Core.Services.SettingsService;
using GameModes.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class PausePopup : BasePopup
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle hapticsToggle;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button resumeButton;

        private SceneService sceneService;
        private SettingsData settingsData;
    
        protected override void Awake()
        {
            base.Awake();
            
            sceneService = ServiceLocator.Get<SceneService>();

            closeButton.onClick.AddListener(OnCloseClicked);
            sfxToggle.onValueChanged.AddListener(SettingsService.SetSFXVolume);
            musicToggle.onValueChanged.AddListener(SettingsService.SetMusicVolume);
            hapticsToggle.onValueChanged.AddListener(SettingsService.SetHaptics);
            homeButton.onClick.AddListener(OnBackToMenuClicked);
            retryButton.onClick.AddListener(OnRetryClicked);
            resumeButton.onClick.AddListener(OnResumeClicked);
        }

        private void OnEnable()
        {
            settingsData ??= SettingsService.Data;
            
            sfxToggle.SetIsOnWithoutNotify(settingsData.sfXVolume > 0);
            musicToggle.SetIsOnWithoutNotify(settingsData.musicVolume > 0);
            hapticsToggle.SetIsOnWithoutNotify(settingsData.hapticsEnabled);
        }

        private void OnCloseClicked()
        {
            Close(true);
            (PopupData as PausePopupData)?.OnResume?.Invoke();
        }

        private void OnRetryClicked()
        {
            Close(true);
            GameSession.Current?.Retry();
        }

        private void OnResumeClicked()
        {
            Close(true);
            (PopupData as PausePopupData)?.OnResume?.Invoke();
        }

        private void OnBackToMenuClicked()
        {
            Close(true);
            Time.timeScale = 1f;
            sceneService.LoadScene(GameConstants.MAIN_MENU_SCENE);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            closeButton.onClick.RemoveListener(OnCloseClicked);
            sfxToggle.onValueChanged.RemoveListener(SettingsService.SetSFXVolume);
            musicToggle.onValueChanged.RemoveListener(SettingsService.SetMusicVolume);
            hapticsToggle.onValueChanged.RemoveListener(SettingsService.SetHaptics);
            homeButton.onClick.RemoveListener(OnBackToMenuClicked);
            retryButton.onClick.RemoveListener(OnRetryClicked);
            resumeButton.onClick.RemoveListener(OnResumeClicked);
        }
        
        private ISettingsService settingsService;
        private ISettingsService SettingsService => settingsService ??= ServiceLocator.Get<ISettingsService>();
    }

    public class PausePopupData : PopupData
    {
        public Action OnResume;
        
        public PausePopupData(Priority priority, Action onResume) : base(priority)
        {
            OnResume = onResume;
        }
    }
}
