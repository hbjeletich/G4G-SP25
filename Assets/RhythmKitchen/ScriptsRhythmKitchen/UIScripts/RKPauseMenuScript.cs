using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class RKPauseMenuScript : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public TMP_Text countdownText;
    public GameObject pausePanel;
    public GameObject judgementLine;
    public GameObject outlines;
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public GameObject notesRuntime; 

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
        clickButton();

        AudioListener.pause = false;

        SceneManager.LoadScene("RKMainMenu");
    }

    public void ResumeGameplay()
    {
        clickButton();

        pausePanel.SetActive(false);
        
        StartCoroutine(countdownStart());
    }

    IEnumerator countdownStart()
    {
        int countdownLength = 3;

        for(int i = countdownLength; i > 0; i--)
        {
            countdownText.text = "" + i;
            yield return new WaitForSeconds(1);
        }

        countdownText.text = "Go!";
        yield return new WaitForSeconds(1);

        AudioListener.pause = false;
        judgementLine.SetActive(true);
        outlines.SetActive(true);
        pausePanel.SetActive(true);
        pauseMenu.SetActive(false);
        notesRuntime.SetActive(true); 
        pauseButton.SetActive(true);
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
