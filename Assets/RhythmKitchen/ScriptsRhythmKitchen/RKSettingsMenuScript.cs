using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class RKSettingsMenuScript : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    void Start()
    {
        float volume;

        audioMixer.GetFloat("RKVolume", out volume);
        volumeSlider.SetValueWithoutNotify(volume);
    }

    public void SetVolume(float volume)
    {
        Debug.Log("volume: " + volume);
        audioMixer.SetFloat("RKVolume", volume);
    }
}
