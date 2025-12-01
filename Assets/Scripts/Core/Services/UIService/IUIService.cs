namespace Core.Services.UIService
{
    public interface IUIService : IService
    {
        void ShowLoadingOverlay(bool show);
    }
}
