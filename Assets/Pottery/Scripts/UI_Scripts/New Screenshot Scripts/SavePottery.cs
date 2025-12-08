using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePottery : MonoBehaviour
{
    public PotteryScreenshot screenshotManager;

    public void OnSaveButtonClick()
    {
        screenshotManager.CaptureScreenshot();
        Debug.Log("Screenshot saved. You can view it in the gallery scene.");
    }
}

