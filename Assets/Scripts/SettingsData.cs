using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public float musicVolume = 1f;
    public float sfXVolume = 1f;
    public bool hapticsEnabled = true;
    public bool notificationsEnabled = true;
    public bool autoPauseEnabled = true;
    public string language = "en";

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
