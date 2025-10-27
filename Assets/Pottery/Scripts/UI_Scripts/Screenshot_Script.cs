using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PotteryScreenshot : MonoBehaviour
{
    public Camera captureCamera; // assign your capture cam in the Inspector
    public int resolution = 512; // 512x512 is fine for thumbnails

    public void CaptureAndSave(string fileName)
    {
        RenderTexture rt = new RenderTexture(resolution, resolution, 24);
        captureCamera.targetTexture = rt;

        Texture2D screenshot = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
        captureCamera.Render();

        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        screenshot.Apply();

        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenshot.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, fileName + ".png");
        File.WriteAllBytes(path, bytes);

        Debug.Log($"Saved screenshot: {path}");
    }
}