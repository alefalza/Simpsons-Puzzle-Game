using Core;
using Core.Services.SettingsService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.Tabs.Settings
{
    public class SettingsTabView : TabView
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Toggle notificationsToggle;
        [SerializeField] private Toggle hapticsToggle;
        [SerializeField] private Toggle autoPauseToggle;
        [SerializeField] private TMP_Dropdown languageDropdown;

        private SettingsService settingsManager;

        private void Awake()
        {
            settingsManager = ServiceLocator.Get<SettingsService>();
        }

        private void Start()
        {
            LoadUIFromData();

            // Listeners
            musicSlider.onValueChanged.AddListener(settingsManager.SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(settingsManager.SetSFXVolume);
            hapticsToggle.onValueChanged.AddListener(settingsManager.SetHaptics);
            notificationsToggle.onValueChanged.AddListener(settingsManager.SetNotifications);
            autoPauseToggle.onValueChanged.AddListener(settingsManager.SetAutoPause);
            languageDropdown.onValueChanged.AddListener(OnLanguageSelected);
        }

        private void LoadUIFromData()
        {
            var data = settingsManager.Data;

            musicSlider.SetValueWithoutNotify(data.MusicVolume);
            sfxSlider.SetValueWithoutNotify(data.SFXVolume);
            hapticsToggle.SetIsOnWithoutNotify(data.HapticsEnabled);
            notificationsToggle.SetIsOnWithoutNotify(data.NotificationsEnabled);
            autoPauseToggle.SetIsOnWithoutNotify(data.AutoPauseEnabled);

            // Language mapping
            int index = languageDropdown.options.FindIndex(o =>
                o.text.ToLower().StartsWith(data.Language.ToLower()));

            if (index >= 0)
                languageDropdown.SetValueWithoutNotify(index);
        }

        private void OnLanguageSelected(int index)
        {
            string code = languageDropdown.options[index].text.ToLower() switch
            {
                "english" => "en",
                "español" => "es",
                _ => "en"
            };

            settingsManager.SetLanguage(code);
        }
    }
}
