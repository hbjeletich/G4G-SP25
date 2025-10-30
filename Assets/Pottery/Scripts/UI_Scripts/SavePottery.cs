using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePotteryButton : MonoBehaviour
{
    private PotteryScreenshot screenshotSystem;

    void Start()
    {
        screenshotSystem = FindObjectOfType<PotteryScreenshot>();
        if (screenshotSystem == null)
            Debug.LogError("[SavePotteryButton] No PotteryScreenshot found in scene.");
    }

    // Hook this to Button -> OnClick()
    public void OnSaveButtonPressed()
    {
        if (screenshotSystem == null) return;

        string baseName = "pottery"; // change to player-provided name if needed
        string savedPath = screenshotSystem.CaptureAndSave(baseName);

        if (!string.IsNullOrEmpty(savedPath))
        {
            Debug.Log("[SavePotteryButton] Save successful: " + savedPath);
       
        }
        else
        {
            Debug.LogError("[SavePotteryButton] Save failed.");
        }
    }
}