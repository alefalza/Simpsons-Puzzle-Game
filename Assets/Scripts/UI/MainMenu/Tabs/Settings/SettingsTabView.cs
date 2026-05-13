using Core;
using Core.Services.SettingsService;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.Tabs.Settings
{
    public class SettingsTabView : TabView
    {
        [SerializeField] private Toggle notificationsToggle;
        [SerializeField] private Toggle autoPauseToggle;

        private void Start()
        {
            LoadUIFromData();
            SubscribeToNotifications();
        }
        
        private void LoadUIFromData()
        {
            var data = SettingsService.Data;

            notificationsToggle.SetIsOnWithoutNotify(data.notificationsEnabled);
            autoPauseToggle.SetIsOnWithoutNotify(data.autoPauseEnabled);
        }

        private void SubscribeToNotifications()
        {
            notificationsToggle.onValueChanged.AddListener(SettingsService.SetNotifications);
            autoPauseToggle.onValueChanged.AddListener(SettingsService.SetAutoPause);
        }

        private ISettingsService settingsService;
        private ISettingsService SettingsService => settingsService ??= ServiceLocator.Get<ISettingsService>();
    }
}
