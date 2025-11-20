using UnityEngine;

namespace UI.Settings
{
    [System.Serializable]
    public class SettingsData
    {
        public float MusicVolume = 1f;
        public float SFXVolume = 1f;
        public bool HapticsEnabled = true;
        public bool NotificationsEnabled = true;
        public bool AutoPauseEnabled = true;
        public string Language = "en";

        private const string KEY = "SETTINGS_DATA";

        public void Save()
        {
            string json = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(KEY, json);
            PlayerPrefs.Save();
        }

        public static SettingsData Load()
        {
            if (!PlayerPrefs.HasKey(KEY))
                return new SettingsData();

            string json = PlayerPrefs.GetString(KEY);
            return JsonUtility.FromJson<SettingsData>(json);
        }
    }
}
