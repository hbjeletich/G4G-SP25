using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    public Renderer targetRenderer; // Assign the object with the mesh renderer
    public Material[] materials;   // Assign materials in the Inspector

    // Call this method when a button is pressed
    public void SwitchMaterial(int materialIndex)
    {
        if (materials != null && materialIndex >= 0 && materialIndex < materials.Length)
        {
            targetRenderer.material = materials[materialIndex];
        }
        else
        {
            Debug.LogWarning("Invalid material index or materials array is empty!");
        }
    }
}

