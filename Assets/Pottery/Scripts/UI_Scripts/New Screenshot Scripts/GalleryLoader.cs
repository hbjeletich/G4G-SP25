using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class GalleryLoader : MonoBehaviour
{
    [Header("Assign up to 6 RawImages")]
    public RawImage[] gallerySlots = new RawImage[6];

    private string screenshotsFolder;
    private const int MaxScreenshots = 6;

    private void Start()
    {
        screenshotsFolder = Path.Combine(Application.persistentDataPath, "PotteryScreenshots");
        Debug.Log("Looking for screenshots in: " + screenshotsFolder);
        LoadGallery(screenshotsFolder);
    }

    public void LoadGallery(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("Screenshot folder missing: " + folderPath);
            return;
        }

        // Clear existing textures
        foreach (var slot in gallerySlots)
            if (slot) slot.texture = null;

        string[] files = Directory.GetFiles(folderPath, "*.png")
                                  .OrderByDescending(f => File.GetCreationTime(f))
                                  .Take(MaxScreenshots)
                                  .ToArray();

        Debug.Log("Found " + files.Length + " screenshots.");

        for (int i = 0; i < files.Length && i < gallerySlots.Length; i++)
        {
            string file = files[i];
            RawImage slot = gallerySlots[i];

            if (slot == null) continue;

            byte[] bytes = File.ReadAllBytes(file);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes))
            {
                Debug.LogWarning("Could not load: " + file);
                continue;
            }

            texture.Apply();
            slot.texture = texture;

            Debug.Log("Loaded screenshot into slot " + i + ": " + Path.GetFileName(file));
        }
    }
}