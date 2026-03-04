using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
    public Slider progressSlider;
    private bool isVisible = true;

    void Start()
    {
        if (progressSlider == null)
        {
            progressSlider = GetComponent<Slider>();
        }

        Hide();
    }

    public void Show()
    {
        if (progressSlider != null && !isVisible)
        {
            foreach(Transform child in progressSlider.transform)
            {
                child.gameObject.SetActive(true);
            }
            isVisible = true;
        }
    }

    public void Hide()
    {
        if (progressSlider != null && isVisible)
        {
            foreach(Transform child in progressSlider.transform)
            {
                child.gameObject.SetActive(false);
            }
            isVisible = false;
        }
    }

    public void SetProgress(float progress)
    {
        if (progressSlider != null)
        {
            progressSlider.value = Mathf.Clamp01(progress);
        }
    }
}
