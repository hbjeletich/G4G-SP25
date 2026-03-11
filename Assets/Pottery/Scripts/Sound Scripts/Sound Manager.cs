using UnityEngine;
using System.Collections;

//Sound Manager Class creation
public class SoundManager : MonoBehaviour
{
    //Headers for Unity UI of script
    [Header("--------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("--------- Audio Clip ----------")]
    public AudioClip menu;
    public AudioClip button;
    public AudioClip background;
    public AudioClip etch;
    public AudioClip paint;
    public AudioClip spinning;

    //Start function for Music that can be attatched to buttons in Unity
    private void Start()
    {
        musicSource.clip = menu;
        musicSource.Play();
    }

    //SFX Function which can be attatched to buttons in Unity
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    //Fade out function to transition between music smoothly
    //Can be attatched to buttons in Unity
    public void FadeOutMusic(float duration = 1f)
    {
        StartCoroutine(FadeOutMusicCoroutine(duration));
    }

    //Logic for Fade Out function
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