using GameModes.DonutStack.Gameplay;
using UI;

namespace GameModes.DonutStack.UI
{
    public class DonutStackHUDController : BaseHUDController
    {
        protected override void OnResumeClicked()
        {
            DonutStackGameManager.Instance.TogglePauseFromOverlay();
        }
    }
}
