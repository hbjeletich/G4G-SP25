using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Sewing
{
    public enum SoundType
    {
        EMBELSHING,
        FOOTSTEPS,
        PUZZLESNAP,
        REWARDONE,
        REWARDTWO,
        SCISSORS,
        SCISSORSTWO,
        SCISSORSTHREE,
        SEWING,
        STOREDOOR
    }

    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] soundlist;
        private static SoundManager instance;
        private AudioSource audioSource;
        private int scissorsIndex = 0;
        
        public AudioClip backgroundMusic;
        private bool playBgMusic = true;

        private HashSet<SoundType> currentlyPlaying = new HashSet<SoundType>();

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (GameSelect.Instance != null)
            {
                if (!GameSelect.Instance.localSource.isPlaying && playBgMusic)
                {
                    GameSelect.Instance.localSource.PlayOneShot(backgroundMusic);
                }
            }
        }

        public static void StartBGM()
        {
            instance.playBgMusic = true;
        }

        public static void StopBGM()
        {
            instance.playBgMusic = false;
            GameSelect.Instance.localSource.Stop();
        }

        public static void PlaySound(SoundType sound, float volume = 1)
        {
            SoundType actualSoundToPlay = sound;

            if (sound == SoundType.SCISSORS)
            {
                SoundType[] scissorsSounds = { SoundType.SCISSORS, SoundType.SCISSORSTWO, SoundType.SCISSORSTHREE };
                actualSoundToPlay = scissorsSounds[instance.scissorsIndex];

                instance.scissorsIndex = (instance.scissorsIndex + 1) % 3;
            }

            if (instance.currentlyPlaying.Contains(actualSoundToPlay))
            {
                return;
            }

            instance.currentlyPlaying.Add(actualSoundToPlay);
            if(instance.audioSource != null) instance.audioSource.PlayOneShot(instance.soundlist[(int)actualSoundToPlay], volume);
            instance.StartCoroutine(instance.RemoveSoundAfterDelay(actualSoundToPlay, instance.soundlist[(int)actualSoundToPlay].length));
        }

        public static void PlayLoopingSound(SoundType sound, float volume = 1)
        {
            StopLoopingSound();

            instance.audioSource.clip = instance.soundlist[(int)sound];
            instance.audioSource.volume = volume;
            instance.audioSource.Play();
        }

        public static void StopLoopingSound()
        {
            instance.audioSource.Stop();
        }

        private IEnumerator RemoveSoundAfterDelay(SoundType sound, float delay)
        {
            yield return new WaitForSeconds(delay);
            currentlyPlaying.Remove(sound);
        }
    }
}
