using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class PotteryScreenshot : MonoBehaviour
{
    public Canvas confirmationCanvas; // assign confirmation UI Canvas
    public string screenshotsFolder;
    public int maxScreenshots = 6;

    private void Awake()
    {
        screenshotsFolder = Path.Combine(Application.persistentDataPath, "PotteryScreenshots");
        if (!Directory.Exists(screenshotsFolder))
            Directory.CreateDirectory(screenshotsFolder);
    }

    public void CaptureScreenshot()
    {
        if (confirmationCanvas != null)
            confirmationCanvas.enabled = false; // hide UI

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filePath = Path.Combine(screenshotsFolder, $"Pottery_{timestamp}.png");

        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log($"Screenshot saved: {filePath}");

        Invoke(nameof(ReenableUI), 0.1f);
        Invoke(nameof(CleanUpOldScreenshots), 0.5f);
    }

    private void ReenableUI()
    {
        if (confirmationCanvas != null)
            confirmationCanvas.enabled = true;
    }

    public void CleanUpOldScreenshots()
    {
        var files = new DirectoryInfo(screenshotsFolder)
            .GetFiles("*.png")
            .OrderByDescending(f => f.CreationTime)
            .ToList();

        for (int i = maxScreenshots; i < files.Count; i++)
        {
            try
            {
                files[i].Delete();
                Debug.Log($"Deleted old screenshot: {files[i].Name}");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to delete {files[i].Name}: {ex.Message}");
            }
        }
    }

    public string[] GetLatestScreenshots()
    {
        return new DirectoryInfo(screenshotsFolder)
            .GetFiles("*.png")
            .OrderByDescending(f => f.CreationTime)
            .Take(maxScreenshots)
            .Select(f => f.FullName)
            .ToArray();
    }
}
