using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishing
{
    public class FishingAudioManager : MonoBehaviour
    {
        #region Top/Components
        public static FishingAudioManager instance;

        [Header("Default Audio Clips")]
        [SerializeField] private AudioClip defaultMusic;
        [SerializeField] private AudioClip defaultAmbient;

        [Header("SFX Clips")]
        public AudioClip castHookSFX;
        public AudioClip reelInSFX;
        public AudioClip regFishCaughtSFX;
        public AudioClip bigFishCaughtSFX;
        public AudioClip bubblesSFX;
        public AudioClip splashSFX;

        private AudioSource musicSource;
        private AudioSource ambientSource;
        private List<AudioSource> sfxSources = new List<AudioSource>();
        private AudioClip currentMusic;
        private AudioClip currentAmbient;
        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            AudioSource[] sources = GetComponents<AudioSource>();

            if (sources.Length >= 5)
            {
                musicSource = sources[0];
                ambientSource = sources[2];

                sfxSources.Clear();
                sfxSources.Add(sources[1]); // sfx1
                sfxSources.Add(sources[3]); // sfx2
                sfxSources.Add(sources[4]); // sfx3
            }
            else
            {
                Debug.LogError("FishingAudioManager: You must have at least 5 AudioSources assigned (music, sfx1, ambient, sfx2, sfx3)");
            }
            StartCoroutine(PlayDefaultAudioDelayed());
            StartCoroutine(PlayRandomBubblesRoutine());
        }

        private IEnumerator PlayDefaultAudioDelayed()
        {
            yield return new WaitForSeconds(0.1f);
            if (defaultMusic != null) PlayMusic(defaultMusic);
            if (defaultAmbient != null) PlayAmbientLoop(defaultAmbient);
        }

        #region Music Methods
        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || clip == currentMusic) return;

            currentMusic = clip;
            musicSource.clip = clip;
            StartCoroutine(FadeIn(musicSource, 1f));
        }

        public void StopMusic()
        {
            StartCoroutine(FadeOut(musicSource, 1f));
            currentMusic = null;
        }

        public void PauseMusic() => musicSource.Pause();
        public void ResumeMusic() => musicSource.UnPause();
        public void SetMusicVolume(float volume) => musicSource.volume = Mathf.Clamp01(volume);
        #endregion

        #region Ambient Methods
        public void PlayAmbientLoop(AudioClip clip)
        {
            if (clip == null || clip == currentAmbient) return;

            currentAmbient = clip;
            ambientSource.clip = clip;
            StartCoroutine(FadeIn(ambientSource, 1f));
        }

        public void StopAmbient()
        {
            StartCoroutine(FadeOut(ambientSource, 1f));
            currentAmbient = null;
        }

        public void SetAmbientVolume(float volume) => ambientSource.volume = Mathf.Clamp01(volume);
        #endregion

        #region SFX Methods
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;

            AudioSource freeSource = GetFreeSFXSource();
            if (freeSource != null)
            {
                freeSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning("FishingAudioManager: No free SFX source available!");
            }
        }

        public void SetSFXVolume(float volume)
        {
            foreach (var src in sfxSources)
            {
                src.volume = Mathf.Clamp01(volume);
            }
        }

        public void SetSFXPitch(float pitch)
        {
            foreach (var src in sfxSources)
            {
                src.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
            }
        }

        public void StopAllSFX()
        {
            foreach (var src in sfxSources)
            {
                src.Stop();
            }
        }

        private AudioSource GetFreeSFXSource()
        {
            foreach (var source in sfxSources)
            {
                if (!source.isPlaying)
                    return source;
            }
            return null;
        }
        #endregion

        #region Fade Methods
        private IEnumerator FadeIn(AudioSource source, float duration)
        {
            float targetVolume = source.volume;
            source.volume = 0;
            source.Play();

            while (source.volume < targetVolume)
            {
                source.volume += targetVolume * Time.deltaTime / duration;
                yield return null;
            }

            source.volume = targetVolume;
        }

        private IEnumerator FadeOut(AudioSource source, float duration)
        {
            float startVolume = source.volume;

            while (source.volume > 0)
            {
                source.volume -= startVolume * Time.deltaTime / duration;
                yield return null;
            }

            source.Stop();
            source.volume = startVolume;
        }
        #endregion

        #region Bubbles
        private IEnumerator PlayRandomBubblesRoutine()
        {
            while (true)
            {
                float waitTime = Random.Range(20f, 60f);
                yield return new WaitForSeconds(waitTime);

                if (bubblesSFX != null && sfxSources.Count > 0)
                {
                    // Play the bubbles sound effect
                    AudioSource freeSource = GetFreeSFXSource();
                    if (freeSource != null)
                    {
                        freeSource.PlayOneShot(bubblesSFX);
                        SetSFXPitch(Random.Range(0.6f, 1.3f));
                        SetSFXVolume(Random.Range(0.05f, 0.15f));
                        Debug.Log("Playing bubbles!");
                    }
                }
                else
                {
                    Debug.LogWarning("FishingAudioManager: Bubbles SFX is not assigned or no free SFX source available!");
                }
            }
        }
        #endregion
    }
}   