using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoomTransition : MonoBehaviour
{
    public Camera mainCam;
    public CanvasGroup startScreen;
    public CanvasGroup levelScreen;
    public float zoomAmount = 3f;
    public float duration = 1f;

    public GameObject guideCharacter;

    private bool isZooming = false;
    private float startFOV;

    void Start()
    {
        startFOV = mainCam.fieldOfView;

        // level select screen is hidden
        levelScreen.alpha = 0;
        levelScreen.interactable = false;
        levelScreen.blocksRaycasts = false;

    }

    public void OnStartButtonClick()
    {
        if (!isZooming)
        {
            // hide start screen, show level select screen
            startScreen.alpha = 0;
            startScreen.interactable = false;
            startScreen.blocksRaycasts = false;

            // camera zoom 
            StartCoroutine(ZoomIn());
        }
    }

    IEnumerator ZoomIn()
    {
        isZooming = true;
        float elapsed = 0f;
        float targetFOV = startFOV / zoomAmount;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mainCam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / duration);
            yield return null;
        }

        levelScreen.alpha = 1;
        levelScreen.interactable = true;
        levelScreen.blocksRaycasts = true;

        guideCharacter.SetActive(true);

        isZooming = false;
    }
}

