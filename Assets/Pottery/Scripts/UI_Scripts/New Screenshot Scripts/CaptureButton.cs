using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is simply assigned to a button that will then render the screen for a screenshot

public class CaptureButton : MonoBehaviour
{
    [Header("Assign your PotteryScreenshot script here")]
    public PotteryScreenshot PotteryScreenshot;

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
