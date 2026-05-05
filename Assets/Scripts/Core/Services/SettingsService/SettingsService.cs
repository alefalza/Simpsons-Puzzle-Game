using Core.Services.AudioService;
using UnityEngine;

namespace Core.Services.SettingsService
{
    public class SettingsService : ISettingsService, IPostInitializableService
    {
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

        public void SetMusicVolume(float value)
        {
            Data.musicVolume = value;
            AudioService.SetMusicVolume(value);
            Data.Save();
        }

        public void SetSFXVolume(float value)
        {
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
