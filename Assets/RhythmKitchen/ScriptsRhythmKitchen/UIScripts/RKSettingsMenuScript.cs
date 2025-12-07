using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Worked on by: Leia Phillips
// Commented by: Leia Phillips
public class RKSettingsMenuScript : MonoBehaviour
{
    public AudioMixer audioMixer; // the audioMixer, set in Unity
    public Slider volumeSlider; // the Slider for volume, set in Unity

    void Start()
    {
        float volume; // the volume

        audioMixer.GetFloat("RKVolume", out volume); // sets variable volume to the current volume value of the audioMixer
        volumeSlider.SetValueWithoutNotify(volume); // sets the slider value to volume, this makes the slider value always correct even after changing scenes
    }

    // Sets the game volume
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("RKVolume", volume); // sets audioMixer volume to variable volume
    }

    public void BackMainMenu()
    {
        // Changing the scene happens in Unity
        RKAudioManager.Instance.PlaySFX("ButtonPress"); // Plays the ButtonPress sfx
    }
}
