using Core;
using Core.Services.PopupService;
using Core.Services.SceneService;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Popups
{
    public class GameOverPopup : BasePopup
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

        public override void Open()
        {
            scoreText.text = $"Final Score: {(PopupData as GameOverPopupData)?.FinalScore}";
            
            base.Open();
        }

        private void OnRetryClicked()
        {
            sceneService.LoadScene(currentScene);
            Close();
        }

        private void OnBackToMenuClicked()
        {
            sceneService.LoadScene(GameConstants.MAIN_MENU_SCENE);
            Close(true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            retryButton.onClick.RemoveListener(OnRetryClicked);
            menuButton.onClick.RemoveListener(OnBackToMenuClicked);
        }
    }

    public class GameOverPopupData : PopupData
    {
        public int FinalScore { get; private set; }
        
        public GameOverPopupData(Priority priority, int finalScore) : base(priority)
        {
            FinalScore = finalScore;
        }
    }
}
