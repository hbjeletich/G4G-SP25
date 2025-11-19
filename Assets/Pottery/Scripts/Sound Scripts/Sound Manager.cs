using UnityEngine;

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

}