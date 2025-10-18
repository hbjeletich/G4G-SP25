using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    public Renderer targetRenderer; // Assign the object with the mesh renderer
    public Material[] materials;   // Assign materials in the Inspector


    //[header("Decal Color Selection")]
    private bool isDecalRed = false;
    private bool isDecalGreen = false;
    private bool isDecalBlue= false;

    [Header("Assign decal materials here")]
    public Material redDecalOMaterial;
    public Material redDecalSMaterial;

    public Material greenDecalOMaterial;
    public Material greenDecalSMaterial;

    public Material blueDecalOMaterial;
    public Material blueDecalSMaterial;




    public static int selectedMaterialIndex = 0; // <-- persists across scenes
    public static Material selectedODecal;
    public static Material selectedSDecal;

    void Start()
    {
        // Apply the previously selected material
        if (targetRenderer != null && materials != null && materials.Length > 0)
        {
            targetRenderer.material = materials[selectedMaterialIndex];
        }
    }


    // Call this method when a button is pressed



    public void Redtrigger()
    {
        isDecalRed = true;
        isDecalGreen = false;
        isDecalBlue = false;

        Debug.Log("Red True");

    }

    public void Greentrigger()
    {
        isDecalRed = false;
        isDecalGreen = true;
        isDecalBlue = false;

        Debug.Log("Green True");

    }

    public void Bluetrigger()
    {
        isDecalRed = false;
        isDecalGreen = false;
        isDecalBlue = true;

        Debug.Log("Blue True");

    }

    public void SetRedODecals()
    {
        if (!isDecalRed) return;

        if (targetRenderer != null && redDecalOMaterial != null)
        {
            targetRenderer.material = redDecalOMaterial;
            selectedODecal = redDecalOMaterial; // <-- Save for next scene
            Debug.Log("Red O Decal Material applied.");
        }
        else
        {
            Debug.LogWarning("Renderer or redDecalOMaterial is missing!");
        }
    }

    public void SetRedSDecals()
    {
        if (!isDecalRed) return;
        if (targetRenderer != null && redDecalSMaterial != null)
        {
            // Set ONLY the red decal material (overwrite all materials)
            targetRenderer.material = redDecalSMaterial;
            Debug.Log("Red S Decal Material applied.");
        }
        else
        {
            Debug.LogWarning("Renderer or redDecalSMaterial is missing!");
        }
    }

    public void SetGreenODecals()
    {
        if (!isDecalGreen) return;
        if (targetRenderer != null && greenDecalOMaterial != null)
        {
            // Set ONLY the green decal material (overwrite all materials)
            targetRenderer.material = greenDecalOMaterial;
            Debug.Log("Green O Decal Material applied.");
        }
        else
        {
            Debug.LogWarning("Renderer or greenDecalOMaterial is missing!");
        }
    }

    public void SetGreenSDecals()
    {
        if (!isDecalGreen) return;
        if (targetRenderer != null && greenDecalSMaterial != null)
        {
            // Set ONLY the green decal material (overwrite all materials)
            targetRenderer.material = greenDecalSMaterial;
            Debug.Log("Green S Decal Material applied.");
        }
        else
        {
            Debug.LogWarning("Renderer or greenDecalSMaterial is missing!");
        }
    }

    public void SetBlueODecals()
    {
        if (!isDecalBlue) return;
        if (targetRenderer != null && blueDecalOMaterial != null)
        {
            // Set ONLY the blue decal material (overwrite all materials)
            targetRenderer.material = blueDecalOMaterial;
            Debug.Log("Blue O Decal Material applied.");
        }
        else
        {
            Debug.LogWarning("Renderer or blueDecalOMaterial is missing!");
        }
    }

    public void SetBlueSDecals()
    {
        if (!isDecalBlue) return;
        if (targetRenderer != null && blueDecalSMaterial != null)
        {
            // Set ONLY the blue decal material (overwrite all materials)
            targetRenderer.material = blueDecalSMaterial;
            Debug.Log("Blue S Decal Material applied.");
        }
        else
        {
            Debug.LogWarning("Renderer or blueDecalSMaterial is missing!");
        }
    }






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

