using UnityEngine;

namespace Core.Services.AudioService
{
    public interface IAudioService : IService
    {
        void PlayMusic(AudioClip clip, float volume = 1f);
        void PlaySFX(AudioClip clip, float volume = 1f);
        void SetMusicVolume(float volume);
        void SetSFXVolume(float volume);
    }
}
