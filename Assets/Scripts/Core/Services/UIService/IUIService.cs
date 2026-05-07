using UI.MainMenu;

namespace Core.Services.UIService
{
    public interface IUIService : IService
    {
        public MainMenuTab LastTabSeen { get; }
        void ShowLoadingOverlay(bool show);
        void UpdateLoadingBar(float progress);
        void SetLastTabSeen(MainMenuTab tab);
    }
}
