using Core;
using Core.Services.AudioService;
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

        private void Start()
        {
            LoadUIFromData();
            SubscribeToNotifications();
            AudioService.SetMusicVolume(musicSlider.value); // TODO: set earlier
            AudioService.SetSFXVolume(sfxSlider.value); // TODO: set earlier
        }
        
        private void LoadUIFromData()
        {
            var data = SettingsService.Data;

            musicSlider.SetValueWithoutNotify(data.musicVolume);
            sfxSlider.SetValueWithoutNotify(data.sfXVolume);
            hapticsToggle.SetIsOnWithoutNotify(data.hapticsEnabled);
            notificationsToggle.SetIsOnWithoutNotify(data.notificationsEnabled);
            autoPauseToggle.SetIsOnWithoutNotify(data.autoPauseEnabled);

            // Language mapping
            int index = languageDropdown.options.FindIndex(o =>
                o.text.ToLower().StartsWith(data.language.ToLower()));

            if (index >= 0)
                languageDropdown.SetValueWithoutNotify(index);
        }

        private void SubscribeToNotifications()
        {
            musicSlider.onValueChanged.AddListener(SettingsService.SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SettingsService.SetSFXVolume);
            hapticsToggle.onValueChanged.AddListener(SettingsService.SetHaptics);
            notificationsToggle.onValueChanged.AddListener(SettingsService.SetNotifications);
            autoPauseToggle.onValueChanged.AddListener(SettingsService.SetAutoPause);
            languageDropdown.onValueChanged.AddListener(OnLanguageSelected);
        }

        private void OnLanguageSelected(int index)
        {
            string code = languageDropdown.options[index].text.ToLower() switch
            {
                "english" => "en",
                "español" => "es",
                _ => "en"
            };

            SettingsService.SetLanguage(code);
        }

        private ISettingsService settingsService;
        private ISettingsService SettingsService => settingsService ??= ServiceLocator.Get<ISettingsService>();
        
        private IAudioService audioService;
        private IAudioService AudioService => audioService ??= ServiceLocator.Get<IAudioService>();
    }
}
