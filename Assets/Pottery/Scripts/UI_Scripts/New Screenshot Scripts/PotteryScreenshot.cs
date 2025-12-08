using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class PotteryScreenshot : MonoBehaviour
{
    private string screenshotsFolder;
    private const int MaxScreenshots = 6;

    private void Awake()
    {
        screenshotsFolder = Path.Combine(Application.persistentDataPath, "PotteryScreenshots");
        if (!Directory.Exists(screenshotsFolder))
            Directory.CreateDirectory(screenshotsFolder);
    }

    public void CaptureScreenshot()
    {
        string fileName = "Pottery_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(screenshotsFolder, fileName);

        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot saved to: " + filePath);

        // Clean up old screenshots after saving
        DeleteOldScreenshots();
    }

    private void DeleteOldScreenshots()
    {
        string[] files = Directory.GetFiles(screenshotsFolder, "*.png");

        // Sort files by creation time (newest first)
        var orderedFiles = files.OrderByDescending(f => File.GetCreationTime(f)).ToList();

        if (orderedFiles.Count > MaxScreenshots)
        {
            // Skip the most recent 6 and delete the rest
            var oldFiles = orderedFiles.Skip(MaxScreenshots);

            foreach (var file in oldFiles)
            {
                try
                {
                    File.Delete(file);
                    Debug.Log("Deleted old screenshot: " + file);
                }
                catch (IOException ex)
                {
                    Debug.LogWarning("Could not delete file: " + file + " (" + ex.Message + ")");
                }
            }
        }
    }

    public string GetScreenshotsFolder()
    {
        return screenshotsFolder;
    }
}