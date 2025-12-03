using UI.MainMenu;

namespace Core.Services.UIService
{
    public interface IUIService : IService
    {
        public MainMenuTab LastTabSeen { get; }
        void ShowLoadingOverlay(bool show);
        void SetLastTabSeen(MainMenuTab tab);
    }
}
