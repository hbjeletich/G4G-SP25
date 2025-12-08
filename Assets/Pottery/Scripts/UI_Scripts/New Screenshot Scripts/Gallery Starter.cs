using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GalleryStarter : MonoBehaviour
{
    public RawImage[] galleryImages;
    public PotteryScreenshot screenshotManager;

    private void Start()
    {
        LoadGallery();
    }

    public void LoadGallery()
    {
        foreach (var img in galleryImages)
            img.texture = null; // Clear existing textures

        string[] screenshots = null;//screenshotManager.GetLatestScreenshots();

        for (int i = 0; i < screenshots.Length && i < galleryImages.Length; i++)
        {
            byte[] imageBytes = File.ReadAllBytes(screenshots[i]);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            galleryImages[i].texture = texture;
        }
    }
}
