using Core;
using Core.Services.PopupService;
using Core.Services.SceneService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class WinPopup : BasePopup
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Button continueButton;

        private SceneService sceneService;

        protected override void Awake()
        {
            base.Awake();

            sceneService = ServiceLocator.Get<SceneService>();

            continueButton.onClick.AddListener(OnContinueClicked);
        }

        public override void Open()
        {
            scoreText.text = $"Final Score: {(PopupData as GameOverPopupData)?.FinalScore}";

            base.Open();
        }

        private void OnContinueClicked()
        {
            sceneService.LoadScene(GameConstants.MAIN_MENU_SCENE);
            Close(true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            continueButton.onClick.RemoveListener(OnContinueClicked);
        }

        public class WinPopupData : PopupData
        {
            public int FinalScore { get; private set; }

            public WinPopupData(Priority priority, int finalScore) : base(priority)
            {
                FinalScore = finalScore;
            }
        }
    }
}
