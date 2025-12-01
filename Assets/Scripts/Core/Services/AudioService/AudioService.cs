using UnityEngine;

namespace Core.Services.AudioService
{
    public class AudioService : IAudioService
    {
        private readonly AudioSource musicSource;
        private readonly AudioSource sfxSource;

        public AudioService(AudioSource music, AudioSource sfx)
        {
            musicSource = music;
            sfxSource = sfx;
        }

        public void Initialize()
        {
            Debug.Log("[AudioService] Initializing...");
            
            if (musicSource == null || sfxSource == null)
            {
                Debug.LogError("[AudioService] Missing AudioSource references!");
            }
        }

        public void PlayMusic(AudioClip clip, float volume = 1f)
        {
            if (musicSource == null) return;
            
            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.Play();
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (sfxSource == null || clip == null) return;
            
            sfxSource.PlayOneShot(clip, volume);
        }

        public void SetMusicVolume(float volume)
        {
            if (musicSource != null)
                musicSource.volume = Mathf.Clamp01(volume);
        }

        public void SetSFXVolume(float volume)
        {
            if (sfxSource != null)
                sfxSource.volume = Mathf.Clamp01(volume);
        }

        public void Shutdown()
        {
            Debug.Log("[AudioService] Shutting down...");
            
            if (musicSource != null)
                musicSource.Stop();
        }
    }
}
