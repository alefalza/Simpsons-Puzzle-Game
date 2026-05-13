using Core;
using Core.Services.PopupService;
using UI.MainMenu;
using UI.Popups;
using UnityEngine;
using UnityEngine.UI;

public class HomeTabView : TabView
{
    [SerializeField] private Button settingsButton;
    [SerializeField] private PopupDefinition settingsPopupDefinition;

    private void Awake()
    {
        settingsButton.onClick.AddListener(ShowPausePopup);
    }

    private void ShowPausePopup()
    {   
        PopupService.Push(settingsPopupDefinition, new SettingsPopupData(settingsPopupDefinition.defaultPriority));
    }

    private IPopupService popupService;
    private IPopupService PopupService => popupService ??= ServiceLocator.Get<IPopupService>();
}
