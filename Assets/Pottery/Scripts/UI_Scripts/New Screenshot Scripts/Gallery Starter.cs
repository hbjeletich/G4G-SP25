using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

// This script takes care of loading the gallery and its images onto the screen. 

public class GalleryStarter : MonoBehaviour
{
    public RawImage[] galleryImages;
    public PotteryScreenshot screenshotManager;

    private void Start() // Function that can be called to take the player to the gallery scene.
    {
        LoadGallery();
    }

    public void LoadGallery()
    {
        foreach (var img in galleryImages)
            img.texture = null; // Clears existting pictures so that old ones do not show up

        string[] screenshots = null; //Gets the latest screenshot;

        // This loop goes through each Raw Image element in the gallery scene and applies the pictures to them.
        // It reads the files, creates a texture out of them, and applies the to the Raw Images
        for (int i = 0; i < screenshots.Length && i < galleryImages.Length; i++) 
        {
            byte[] imageBytes = File.ReadAllBytes(screenshots[i]);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            galleryImages[i].texture = texture;
        }
    }
}
