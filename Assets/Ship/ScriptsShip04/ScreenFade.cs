using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ship
{
public class ScreenFade : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup; 
    public float fadeDuration = 3.0f;

    public void Start() {}

    public IEnumerator BlackFadeIn()
    {
        float timer = 0;
        while (timer <= fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1;
    }

    public IEnumerator BlackFadeOut()
    {
        float timer = 0;
        while (timer <= fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0;
    }

    public IEnumerator BlackFadeOutVillage()
    {
        if (ShipAudioManager.instance != null &&  ShipAudioManager.instance.bellSound != null)
        {
            ShipAudioManager.instance.SetSFXVolume(0.25f);
            ShipAudioManager.instance.PlaySFX(ShipAudioManager.instance.bellSound);
        }
        else
        {
            Debug.LogWarning("ShipAudioManager or bellSound is not assigned.");
        }
        yield return new WaitForSeconds(1f);  // Wait for sound 
        
        float timer = 0;
        while (timer <= fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0;
    }
}
}

