namespace Core.Services.PopupService
{
    public interface IPopupService : IService
    {
        public T Show<T>(PopupDefinition definition) where T : BasePopup;
        public void Close(string popupId);
        public void Close(BasePopup popup);
    }
}
