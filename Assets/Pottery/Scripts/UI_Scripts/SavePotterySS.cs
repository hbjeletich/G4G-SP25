using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePotteryButton : MonoBehaviour
{
    private PotteryScreenshot screenshotSystem;

    void Start()
    {
        // Finds your ScreenshotManager in the scene
        screenshotSystem = FindObjectOfType<PotteryScreenshot>();
    }

    // This function will be called by the Finish button
    public void OnSaveButtonPressed()
    {
        string fileName = "pottery_" + System.DateTime.Now.ToFileTime();
        screenshotSystem.CaptureAndSave(fileName);
    }
}
