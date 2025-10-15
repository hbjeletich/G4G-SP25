using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    public Renderer targetRenderer; // Assign the object with the mesh renderer
    public Material[] materials;   // Assign materials in the Inspector


    public static int selectedMaterialIndex = 0; // <-- persists across scenes

    void Start()
    {
        // Apply the previously selected material
        if (targetRenderer != null && materials != null && materials.Length > 0)
        {
            targetRenderer.material = materials[selectedMaterialIndex];
        }
    }


    // Call this method when a button is pressed
    public void SwitchMaterial(int materialIndex)
    {
        if (materials != null && materialIndex >= 0 && materialIndex < materials.Length)
        {

            selectedMaterialIndex = materialIndex; // Store the selection globally
            targetRenderer.material = materials[materialIndex];
        }
        else
        {
            Debug.LogWarning("Invalid material index or materials array is empty!");
        }
    }
}

