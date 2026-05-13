using Core;
using Core.Services.PopupService;
using Core.Services.SettingsService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class SettingsPopup : BasePopup
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle hapticsToggle;
        [SerializeField] private TMP_Dropdown languageDropdown;

        private SettingsData settingsData;
        
        protected override void Awake()
        {
            base.Awake();

            closeButton.onClick.AddListener(OnCloseClicked);
            sfxToggle.onValueChanged.AddListener(SettingsService.SetSFXVolume);
            musicToggle.onValueChanged.AddListener(SettingsService.SetMusicVolume);
            hapticsToggle.onValueChanged.AddListener(SettingsService.SetHaptics);
            languageDropdown.onValueChanged.AddListener(OnLanguageSelected);
        }

        private void OnEnable()
        {
            settingsData ??= SettingsService.Data;
            
            sfxToggle.SetIsOnWithoutNotify(settingsData.sfXVolume > 0);
            musicToggle.SetIsOnWithoutNotify(settingsData.musicVolume > 0);
            hapticsToggle.SetIsOnWithoutNotify(settingsData.hapticsEnabled);
            
            // Language mapping
            int index = languageDropdown.options.FindIndex(o =>
                o.text.ToLower().StartsWith(settingsData.language.ToLower()));

            if (index >= 0)
                languageDropdown.SetValueWithoutNotify(index);
        }

        private void OnCloseClicked()
        {
            Close();
        }
        
        private void OnLanguageSelected(int index)
        {
            string code = languageDropdown.options[index].text.ToLower() switch
            {
                "english" => "en",
                "spanish" => "es",
                "french" => "fr",
                "italian" => "it",
                _ => "en"
            };

            SettingsService.SetLanguage(code);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            closeButton.onClick.RemoveListener(OnCloseClicked);
            sfxToggle.onValueChanged.RemoveListener(SettingsService.SetSFXVolume);
            musicToggle.onValueChanged.RemoveListener(SettingsService.SetMusicVolume);
            hapticsToggle.onValueChanged.RemoveListener(SettingsService.SetHaptics);
            languageDropdown.onValueChanged.RemoveListener(OnLanguageSelected);
        }
        
        private ISettingsService settingsService;
        private ISettingsService SettingsService => settingsService ??= ServiceLocator.Get<ISettingsService>();
    }
    
    public class SettingsPopupData : PopupData
    {
        public SettingsPopupData(Priority priority) : base(priority)
        {
            
        }
    }
}
