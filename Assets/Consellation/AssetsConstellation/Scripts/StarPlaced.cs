using UnityEngine;

namespace Constellation
{
    public class StarSoundPlayer : MonoBehaviour
    {
        [Header("Star References")]
        [Tooltip("Drag all star GameObjects here")]
        public GameObject[] stars;

        [Header("Audio Settings")]
        [Tooltip("The sound to play when any star finds its home")]
        public AudioClip starPlacedSound;
        
        [Range(0f, 1f)]
        public float volume = 1f;

        private AudioSource audioSource;
        private bool[] previousFoundHome;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            audioSource.playOnAwake = false;

            previousFoundHome = new bool[stars.Length];
            
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    StarScript starScript = stars[i].GetComponent<StarScript>();
                    if (starScript != null)
                    {
                        previousFoundHome[i] = starScript.foundHome;
                    }
                }
            }
        }

        void Update()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    StarScript starScript = stars[i].GetComponent<StarScript>();
                    if (starScript != null)
                    {
                        if (starScript.foundHome && !previousFoundHome[i])
                        {
                            PlayStarPlacedSound();
                        }
                        previousFoundHome[i] = starScript.foundHome;
                    }
                }
            }
        }

        void PlayStarPlacedSound()
        {
            if (starPlacedSound != null)
            {
                audioSource.PlayOneShot(starPlacedSound, volume);
            }
        }
    }
}