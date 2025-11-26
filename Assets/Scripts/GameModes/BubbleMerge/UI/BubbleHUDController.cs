using UI;
using UI.Overlays;
using UnityEngine;
using UnityEngine.UI;

namespace GameModes.BubbleMerge.UI
{
    public class BubbleHUDController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private ScoreUI scoreUI;
        [SerializeField] private Image currentBubbleIcon;
        [SerializeField] private Image nextBubbleIcon;

        [Header("Bubble Icons")]
        [SerializeField] private Sprite[] bubbleTierIcons;

        [Header("Overlays")]
        [SerializeField] private PauseOverlay pauseOverlay;
        [SerializeField] private GameOverOverlay gameOverOverlay;

        private PauseOverlay pauseOverlayInstance;
        private GameOverOverlay gameOverOverlayInstance;

        public void UpdateScore(int newScore)
        {
            scoreUI.SetScore(newScore);
        }

        public void UpdateCurrentBubbleIcon(int tier)
        {
            if (tier < 0 || tier >= bubbleTierIcons.Length) return;

            currentBubbleIcon.sprite = bubbleTierIcons[tier];
        }

        public void UpdateNextBubbleIcon(int tier)
        {
            if (tier < 0 || tier >= bubbleTierIcons.Length) return;

            nextBubbleIcon.sprite = bubbleTierIcons[tier];
        }

        public bool CanTogglePause()
        {
            return pauseOverlayInstance == null || !pauseOverlayInstance.IsFading;
        }

        public void ShowPauseOverlay()
        {
            if (pauseOverlayInstance == null)
                pauseOverlayInstance = Instantiate(pauseOverlay);

            pauseOverlayInstance.Show();
        }

        public void HidePauseOverlay()
        {
            pauseOverlayInstance.Hide();
        }

        public void ShowGameOverOverlay(int finalScore)
        {
            gameOverOverlayInstance = Instantiate(gameOverOverlay);
            gameOverOverlayInstance.Show(finalScore);
        }
    }
}
