using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GalleryPuller : MonoBehaviour
{
    public Transform galleryParent; // assign your Grid Layout Group object
    public GameObject imagePrefab; // UI prefab with an Image component

    void Start()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.png");

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