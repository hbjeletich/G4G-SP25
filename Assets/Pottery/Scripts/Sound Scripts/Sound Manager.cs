using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
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

    private void Start()
    {
        musicSource.clip = menu;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void FadeOutMusic(float duration = 1f)
    {
        StartCoroutine(FadeOutMusicCoroutine(duration));
    }

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