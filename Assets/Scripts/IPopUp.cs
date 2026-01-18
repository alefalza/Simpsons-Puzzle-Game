using System;
using Core.Services.PopupService;

public interface IPopUp
{
    PopupData PopupData { get; }
    PopupDefinition Definition { get; }
    bool IsFading { get; }
    bool IsActive { get; }
    event Action OnOpened;
    event Action<bool> OnClosed;
    void Setup(PopupData data, PopupDefinition def);
    void Open();
    void Close(bool immediate = false);
    void SetActive(bool isActive);
}
