namespace Core.Services.PopupService
{
    public interface IPopupService : IService
    {
        void Push(PopupDefinition def, PopupData data);
        BasePopup GetOpenedPopup();
    }
}
