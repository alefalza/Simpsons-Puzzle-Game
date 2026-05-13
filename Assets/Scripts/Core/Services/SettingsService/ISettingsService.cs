namespace Core.Services.SettingsService
{
    public interface ISettingsService : IService
    {
        SettingsData Data { get; }
        public void SetMusicVolume(bool enabled);
        public void SetSFXVolume(bool enabled);
        public void SetHaptics(bool enabled);
        public void SetNotifications(bool enabled);
        public void SetAutoPause(bool enabled);
        public void SetLanguage(string code);
    }
}
