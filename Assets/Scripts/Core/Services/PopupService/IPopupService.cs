namespace Core.Services.PopupService
{
    public interface IPopupService : IService
    {
        void ShowPopup(string popupId);
        void HidePopup(string popupId);
    }
}
