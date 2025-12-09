using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [Header("--------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    //List of audio for when the script is attatched
    [Header("--------- Audio Clip ----------")]
    public AudioClip menu;
    public AudioClip button;
    public AudioClip background;
    public AudioClip etch;
    public AudioClip paint;
    public AudioClip spinning;

    private void Start()
    {
        musicSource.clip = menu;
        musicSource.Play();
    }

    //Below is the function for playing a sound effect, which can be attatched to buttons
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    //The following is a function to fade the music out and can be added to the buttons to transition audios
    public void FadeOutMusic(float duration = 1f)
    {
        StartCoroutine(FadeOutMusicCoroutine(duration));
    }

    //The following is the mechanism behind changing the volume in the FadeOutMusic function
    private IEnumerator FadeOutMusicCoroutine(float duration)
    {
        float startVol = musicSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, time / duration);
            yield return null;
        }

        musicSource.Stop();
    }

}