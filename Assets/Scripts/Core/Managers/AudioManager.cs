using UnityEngine;

namespace Core.Managers
{
    public class AudioManager : MonoBehaviour, IService
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        /// <summary>
        /// Plays a music track and replaces any current one.
        /// </summary>
        public void PlayMusic(AudioClip clip, float volume = 1f)
        {
            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.Play();
        }

        public void SetMusicVolume(float v)
        {
            musicSource.volume = v;
        }
        
        /// <summary>
        /// Plays a sound effect once.
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
        
        public void SetSFXVolume(float v)
        {
            sfxSource.volume = v;
        }
    }
}
