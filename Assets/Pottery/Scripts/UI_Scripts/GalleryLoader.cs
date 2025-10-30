using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class GalleryLoader : MonoBehaviour
{
    public Transform galleryParent; // Grid Layout Group object
    public GameObject imagePrefab; // Prefab with Image component
    public int maxScreenshots = 6; // memory-safe limit

    void Start()
    {
        LoadGallery();
    }

    void LoadGallery()
    {
        // Get all .png files and sort by newest first
        var files = Directory.GetFiles(Application.persistentDataPath, "*.png")
                             .OrderByDescending(f => File.GetCreationTime(f))
                             .Take(maxScreenshots) // limit to 6 most recent
                             .ToArray();

        foreach (var file in files)
        {
            byte[] bytes = File.ReadAllBytes(file);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);

            GameObject newImage = Instantiate(imagePrefab, galleryParent);
            newImage.GetComponent<Image>().sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }
}