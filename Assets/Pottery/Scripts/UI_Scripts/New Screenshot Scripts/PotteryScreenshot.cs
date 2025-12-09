using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class PotteryScreenshot : MonoBehaviour
{
    private string screenshotsFolder; // Variable for setting the file path for screenshots
    private const int MaxScreenshots = 6; // Variable for setting the max number of screenshots saved on the computer

    private void Awake()
    {
        // Builds the folder path for the screenshots
        screenshotsFolder = Path.Combine(Application.persistentDataPath, "PotteryScreenshots");

        // Creates a screenshots folder if one doesn't exist
        if (!Directory.Exists(screenshotsFolder))
            Directory.CreateDirectory(screenshotsFolder);
    }

    public void CaptureScreenshot() // Function to render and capture the screen
    {

        // Creates a unique filename using date and time
        string fileName = "Pottery_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(screenshotsFolder, fileName);

        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot saved to: " + filePath);

        // Deletes old screenshots
        DeleteOldScreenshots();
    }

    private void DeleteOldScreenshots() // Function to delete old screenshots
    {
        string[] files = Directory.GetFiles(screenshotsFolder, "*.png");

        // Sort files by time of creation, with newest picture first
        var orderedFiles = files.OrderByDescending(f => File.GetCreationTime(f)).ToList();

        if (orderedFiles.Count > MaxScreenshots)
        {
            // Skip the most recent 6 and delete the rest
            var oldFiles = orderedFiles.Skip(MaxScreenshots);

            foreach (var file in oldFiles)
            {
                try
                {
                    // Attempts to delete each older file
                    File.Delete(file);
                    Debug.Log("Deleted old screenshot: " + file);
                }
                catch (IOException ex)
                {
                    // In case file is locked or inaccessible
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