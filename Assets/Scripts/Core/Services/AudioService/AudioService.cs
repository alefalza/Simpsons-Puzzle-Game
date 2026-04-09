using UnityEngine;
using UnityEngine.Audio;

namespace Core.Services.AudioService
{
    public class AudioService : IAudioService
    {
        private readonly AudioMixer mainMixer;
        private readonly AudioSource musicSource;
        private readonly AudioSource sfxSource;

        public AudioService(AudioMixer mixer, AudioSource music, AudioSource sfx)
        {
            mainMixer = mixer;
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
            SetMainMixerVolume("MusicVol", volume);
        }

        public void SetSFXVolume(float volume)
        {
            SetMainMixerVolume("SFXVol", volume);
        }

        private void SetMainMixerVolume(string name, float volume)
        {
            float value = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            mainMixer.SetFloat(name, value);
        }

        public void Shutdown()
        {
            Debug.Log("[AudioService] Shutting down...");
            
            if (musicSource != null)
                musicSource.Stop();
        }
    }
}
