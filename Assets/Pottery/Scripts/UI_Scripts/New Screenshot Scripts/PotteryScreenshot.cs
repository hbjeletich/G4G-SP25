using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class PotteryScreenshot : MonoBehaviour
{
    private string screenshotsFolder;
    private const int maxScreenshots = 6;
    public Canvas Canvas;

    private void Awake()
    {
        screenshotsFolder = Path.Combine(Application.persistentDataPath, "PotteryScreenshots");
        if (!Directory.Exists(screenshotsFolder))
            Directory.CreateDirectory(screenshotsFolder);
    }

    public void CaptureScreenshot()
    {
        // Hide UI
        if (Canvas != null) Canvas.enabled = false;

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filePath = Path.Combine(screenshotsFolder, $"Pottery_{timestamp}.png");

        // Schedule screenshot at end of frame
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log($"Scheduled screenshot: {filePath}");

        // Re-enable UI after a short delay
        if (Canvas != null) Invoke(nameof(ReenableUI), 0.1f);

        CleanUpOldScreenshots();
    }

    // Helper to turn UI back on
    private void ReenableUI()
    {
        if (Canvas != null) Canvas.enabled = true;
    }

    private void CleanUpOldScreenshots()
    {
        var files = new DirectoryInfo(screenshotsFolder)
            .GetFiles("*.png")
            .OrderByDescending(f => f.CreationTime)
            .ToList();

        // If there are more than 6, delete the oldest ones
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
