using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    EMBELSHING,
    FOOTSTEPS,
    PUZZLESNAP,
    REWARDONE,
    REWARDTWO,
    SCISSORS,
    sCISSORSTWO,
    SEWING,   
    STOREDOOR
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundlist; 
    private static SoundManager instance;
    private AudioSource audioSource; 
    private void Awake()
    {
        instance = this;  
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public static void PlaySound(SoundType sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundlist[(int)sound], volume);
    }
}
