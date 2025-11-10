using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GalleryStarter : MonoBehaviour
{
    public RawImage[] galleryImages;       // Assign 6 RawImages
    public PotteryScreenshot screenshotManager;

    public void LoadGallery()
    {
        string[] screenshots = screenshotManager.GetLatestScreenshots();

        // Clear old images
        foreach (var r in galleryImages)
            if (r != null) r.texture = null;

        // Load images
        for (int i = 0; i < screenshots.Length && i < galleryImages.Length; i++)
        {
            byte[] bytes = File.ReadAllBytes(screenshots[i]);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            galleryImages[i].texture = tex;
        }
    }
}
