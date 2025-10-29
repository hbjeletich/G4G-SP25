using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class RKPauseMenuScript : MonoBehaviour
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

    public void BackMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        clickButton();

        SceneManager.LoadScene("RKMainMenu");
    }

    public void ResumeGameplay()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        clickButton();
    }

    private void clickButton()
    {
        var am = RKAudioManager.Instance;

        if (am != null)
            am.PlaySFX("ButtonPress");
        else
            Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway.");
    
    }
}
