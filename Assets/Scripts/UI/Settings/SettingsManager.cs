using System;
using Core;
using Core.Managers;
using UnityEngine;

namespace UI.Settings
{
    public class SettingsManager : MonoBehaviour, IService
    {
        public SettingsData Data { get; private set; }

        private AudioManager audioManager;
    
        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            Data = SettingsData.Load();
            audioManager = ServiceLocator.Get<AudioManager>();
        }

        public void SetMusicVolume(float value)
        {
            Data.MusicVolume = value;
            audioManager.SetMusicVolume(value);
            Data.Save();
        }

        public void SetSFXVolume(float value)
        {
            Data.SFXVolume = value;
            audioManager.SetSFXVolume(value);
            Data.Save();
        }

        public void SetHaptics(bool enabled)
        {
            Data.HapticsEnabled = enabled;
            Data.Save();
        }

        public void SetNotifications(bool enabled)
        {
            Data.NotificationsEnabled = enabled;
            Data.Save();
        }

        public void SetAutoPause(bool enabled)
        {
            Data.AutoPauseEnabled = enabled;
            Data.Save();
        }

        public void SetLanguage(string code)
        {
            Data.Language = code;
            // TODO: LocalizationSystem
            Data.Save();
        }
    }
}
