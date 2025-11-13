using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureButton : MonoBehaviour
{
    [Header("Assign your PotteryScreenshot script here")]
    public PotteryScreenshot PotteryScreenshot;

    // This function will be called by the Button's OnClick
    public void OnClickCapture()
    {
        if (PotteryScreenshot != null)
        {
            PotteryScreenshot.CaptureScreenshot();
        }
        else
        {
            Debug.LogWarning("PotteryScreenshot script not assigned!");
        }
    }
}
