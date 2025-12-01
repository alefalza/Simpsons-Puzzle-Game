using GameModes.DonutStack.Gameplay;
using UI;

namespace GameModes.DonutStack.UI
{
    public class HexHUDController : BaseHUDController
    {
        public override void OnResumeClicked()
        {
            HexGameManager.Instance.TogglePauseFromOverlay();
        }
    }
}
