using UI.Overlays;
using UnityEngine;

namespace UI
{
    public interface IHUDController
    {
        public void UpdateScore(int newScore);
        public bool CanTogglePause();
        public void ShowPauseOverlay();
        public void HidePauseOverlay();
        public void ShowGameOverOverlay(int finalScore);
    }
    
    public abstract class BaseHUDController : MonoBehaviour, IHUDController
    {
        [Header("UI")]
        [SerializeField] private ScoreUI scoreUI;
        
        [Header("Overlays")]
        [SerializeField] private PauseOverlay pauseOverlay;
        [SerializeField] private GameOverOverlay gameOverOverlay;

        private PauseOverlay pauseOverlayInstance;
        private GameOverOverlay gameOverOverlayInstance;
        
        private bool IsGameOver => gameOverOverlayInstance != null;
        
        public virtual void UpdateScore(int newScore)
        {
            scoreUI.SetScore(newScore);
        }

        public virtual bool CanTogglePause()
        {
            return (pauseOverlayInstance == null || !pauseOverlayInstance.IsFading) && !IsGameOver;
        }

        public virtual void ShowPauseOverlay()
        {
            if (pauseOverlayInstance == null)
                pauseOverlayInstance = Instantiate(pauseOverlay);

            pauseOverlayInstance.Show();
            pauseOverlayInstance.OnResume += OnResumeClicked;
        }

        protected abstract void OnResumeClicked();

        public virtual void HidePauseOverlay()
        {
            pauseOverlayInstance.Hide();
            pauseOverlayInstance.OnResume -= OnResumeClicked;
        }

        public virtual void ShowGameOverOverlay(int finalScore)
        {
            gameOverOverlayInstance = Instantiate(gameOverOverlay);
            gameOverOverlayInstance.Show(finalScore);
        }
    }
}
