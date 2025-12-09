namespace Core.Services.PopupService
{
    public interface IPopupService : IService
    {
        void PushPopup(PopupData data);
        int GetQueueSize();
        BasePopup GetOpenedPopup();
    }
}
