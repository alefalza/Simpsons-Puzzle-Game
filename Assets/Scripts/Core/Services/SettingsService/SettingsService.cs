using Core.Services.AudioService;
using UnityEngine;

namespace Core.Services.SettingsService
{
    public class SettingsService : ISettingsService, IPostInitializableService
    {
        private const float DEFAULT_MUSIC_VOLUME = 0.5f;
        private const float DEFAULT_SFX_VOLUME = 1f;
        
        public SettingsData Data { get; private set; }

        public SettingsService() { }
        
        public void Initialize()
        {
            Debug.Log("[SettingsService] Initializing...");
            Data = SettingsData.Load();
        }

        public void PostInitialize()
        {
            // Apply persisted settings once all services are ready.
            if (Data == null)
            {
                Data = SettingsData.Load();
            }

            AudioService.SetMusicVolume(Data.musicVolume);
            AudioService.SetSFXVolume(Data.sfXVolume);
        }

        public void SetMusicVolume(bool enabled)
        {
            float value = enabled ? DEFAULT_MUSIC_VOLUME : 0;
            Data.musicVolume = value;
            AudioService.SetMusicVolume(value);
            Data.Save();
        }

        public void SetSFXVolume(bool enabled)
        {
            float value = enabled ? DEFAULT_SFX_VOLUME : 0;
            Data.sfXVolume = value;
            AudioService.SetSFXVolume(value);
            Data.Save();
        }

        public void SetHaptics(bool enabled)
        {
            Data.hapticsEnabled = enabled;
            Data.Save();
        }

        public void SetNotifications(bool enabled)
        {
            Data.notificationsEnabled = enabled;
            Data.Save();
        }

        public void SetAutoPause(bool enabled)
        {
            Data.autoPauseEnabled = enabled;
            Data.Save();
        }

        public void SetLanguage(string code)
        {
            Data.language = code;
            // TODO: LocalizationSystem
            Data.Save();
        }

        public void Shutdown()
        {
            Debug.Log("[SettingsService] Shutting down...");
        }
        
        private IAudioService audioService;
        private IAudioService AudioService => audioService ??= ServiceLocator.Get<IAudioService>();
    }
}
