using GameModes.DonutStack.Gameplay;
using UI;

namespace GameModes.DonutStack.UI
{
    public class HexHUDController : BaseHUDController
    {
        protected override void OnResumeClicked()
        {
            HexGameManager.Instance.TogglePauseFromOverlay();
        }
    }
}
