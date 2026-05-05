namespace Core.Services.SettingsService
{
    public interface ISettingsService : IService
    {
        SettingsData Data { get; }
        public void SetMusicVolume(float value);
        public void SetSFXVolume(float value);
        public void SetHaptics(bool enabled);
        public void SetNotifications(bool enabled);
        public void SetAutoPause(bool enabled);
        public void SetLanguage(string code);
    }
}
