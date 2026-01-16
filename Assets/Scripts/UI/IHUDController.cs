namespace UI
{
    public interface IHUDController
    {
        public void UpdateScore(int newScore);
        public bool CanTogglePause();
        public void ShowPausePopup();
        public void HidePausePopup();
        public void ShowGameOverOverlay(int finalScore);
    }
}
