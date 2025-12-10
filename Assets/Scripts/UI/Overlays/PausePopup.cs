using System;
using Core;
using Core.Services.SceneService;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Overlays
{
    public class PausePopup : BasePopup
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button menuButton;

        private SceneService sceneService;
    
        protected override void Awake()
        {
            base.Awake();
            
            sceneService = ServiceLocator.Get<SceneService>();

            resumeButton.onClick.AddListener(OnResumeClicked);
            menuButton.onClick.AddListener(OnBackToMenuClicked);
        }

        private void OnResumeClicked()
        {
            (PopupData as PausePopupData)?.OnResume?.Invoke();
            Close();
        }

        private void OnBackToMenuClicked()
        {
            sceneService.LoadScene("MainMenuScene");
            Close(true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            resumeButton.onClick.RemoveListener(OnResumeClicked);
            menuButton.onClick.RemoveListener(OnBackToMenuClicked);
        }
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
