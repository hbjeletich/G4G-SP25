using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public class MusicPersistance : MonoBehaviour
    {
        public static MusicPersistance instance;
        [HideInInspector] public AudioSource musicSource, ambientSource;   
        private AudioClip currentMusic, currentAmbient;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                AudioSource[] sources = GetComponents<AudioSource>();
                if (sources.Length >= 3)
                {
                    // Match ShipAudioManager's ordering
                    musicSource = sources[0];
                    ambientSource = sources[2];

                    // looping enabled for music and ambient
                    musicSource.loop = true;
                    ambientSource.loop = true;
                }
                else
                {
                    Debug.LogError("MusicPersistance: You must have at least 3 AudioSources (music, sfx, ambient)");
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CheckMusic(AudioClip newMusic)
        {
            if (newMusic != currentMusic)
            {
                currentMusic = newMusic;
                musicSource.clip = newMusic;
                StartCoroutine(MusicFadeIn(musicSource, 1f));
            }
        }

        public void CheckAmbient(AudioClip newAmbient)
        {
            if (newAmbient != currentAmbient)
            {
                currentAmbient = newAmbient;
                ambientSource.clip = newAmbient;
                StartCoroutine(AmbientFadeIn(ambientSource, 1f));
            }
        }

        public void PreTransitionCheckMusic(AudioClip newMusic)
        {
            if (newMusic != currentMusic)
            {
                StartCoroutine(MusicFadeOut(musicSource, 1f));
            }
        }

        public void PreTransitionCheckAmbient(AudioClip newAmbient)
        {
            if (newAmbient != currentAmbient)
            {
                StartCoroutine(AmbientFadeOut(ambientSource, 1f));
            }
        }

        public void StopMusic()
        {
            musicSource.Stop();
            currentMusic = null;
        }

        public void StopAmbient()
        {
            ambientSource.Stop();
            currentAmbient = null;
        }

        public IEnumerator MusicFadeOut(AudioSource musicSource, float fadeDuration)
        {
            float startVolume = musicSource.volume;

            while (musicSource.volume > 0)
            {
                musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = startVolume;
        }

        public IEnumerator AmbientFadeOut(AudioSource ambientSource, float fadeDuration)
        {
            float startVolume = ambientSource.volume;

            while (ambientSource.volume > 0)
            {
                ambientSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }

            ambientSource.Stop();
            ambientSource.volume = startVolume;
        }

        public IEnumerator MusicFadeIn(AudioSource musicSource, float fadeDuration)
        {
            float startVolume = musicSource.volume;
            musicSource.volume = 0;
            musicSource.Play();

            while (musicSource.volume < startVolume)
            {
                musicSource.volume += startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }

            musicSource.volume = startVolume;
        }

        public IEnumerator AmbientFadeIn(AudioSource ambientSource, float fadeDuration)
        {
            float startVolume = ambientSource.volume;
            ambientSource.volume = 0;
            ambientSource.Play();

            while (ambientSource.volume < startVolume)
            {
                ambientSource.volume += startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }

            ambientSource.volume = startVolume;
        }
    }
}
