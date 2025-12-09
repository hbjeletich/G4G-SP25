using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

// Loads up to 6 recent screenshot images from the PotteryScreenshots folder
// and displays them in assigned RawImage slots in the gallery UI.

public class GalleryLoader : MonoBehaviour
{
    [Header("Assign up to 6 RawImages")]
    public RawImage[] gallerySlots = new RawImage[6]; // Gets the Raw Image slots

    private string screenshotsFolder; // Path of the folder in explorer
    private const int MaxScreenshots = 6; //Assigns maximum number of screenshots

    // Function that sets and debugs for the folder that the screenshots reside in
    private void Start()
    {
        screenshotsFolder = Path.Combine(Application.persistentDataPath, "PotteryScreenshots");
        Debug.Log("Looking for screenshots in: " + screenshotsFolder);
    }


    // Function that loads the gallery screen when called and updates it with new screenshots
    public void UpdateGallery()
    {
        LoadGallery(screenshotsFolder);
    }

    public void LoadGallery(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("Screenshot folder missing: " + folderPath);
            return;
        } //Checks to see if the folder

        // Clears existing textures
        foreach (var slot in gallerySlots)
            if (slot) slot.texture = null;

        // Gets the newest screenshots
        string[] files = Directory.GetFiles(folderPath, "*.png")
                                  .OrderByDescending(f => File.GetCreationTime(f))
                                  .Take(MaxScreenshots)
                                  .ToArray();

        Debug.Log("Found " + files.Length + " screenshots.");

        // This loop loads all the screenshots into the raw image slots
        for (int i = 0; i < files.Length && i < gallerySlots.Length; i++)
        {
            string file = files[i];
            RawImage slot = gallerySlots[i];

            if (slot == null) continue;

            byte[] bytes = File.ReadAllBytes(file);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false); // Reads the files and creates textures

            if (!texture.LoadImage(bytes)) // Load PNG Data
            {
                Debug.LogWarning("Could not load: " + file);
                continue;
            }

            texture.Apply(); //apply textures
            slot.texture = texture; // show image

            Debug.Log("Loaded screenshot into slot " + i + ": " + Path.GetFileName(file));
        }
    }
}