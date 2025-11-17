//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MaterialSwitcher : MonoBehaviour
//{
//    public Renderer targetRenderer; // Assign the object with the mesh renderer
//    public Material[] materials;   // Assign materials in the Inspector


//    //[header("Decal Color Selection")]
//    private bool isDecalRed = false;
//    private bool isDecalGreen = false;
//    private bool isDecalBlue = false;
//    private bool isDecalYellow = false;

//    [Header("Assign decal materials here")]
//    public Material redDecalOMaterial;
//    public Material redDecalSMaterial;

//    public Material greenDecalOMaterial;
//    public Material greenDecalSMaterial;

//    public Material blueDecalOMaterial;
//    public Material blueDecalSMaterial;

//    public Material yellowDecalOMaterial;
//    public Material yellowDecalSMaterial;



//    public static Material selectedMaterial; // ? This will persist between scenes

//    public static int selectedMaterialIndex = 0; // <-- persists across scenes
//    public static Material selectedODecal;
//    public static Material selectedSDecal;

//    void Start()
//    {
//        if (targetRenderer != null)
//        {
//            if (selectedMaterial != null)
//            {
//                targetRenderer.material = selectedMaterial; // ? Load previously applied material
//                Debug.Log("Loaded selected material from previous scene.");
//            }
//            else if (materials != null && materials.Length > 0)
//            {
//                targetRenderer.material = materials[selectedMaterialIndex];
//                selectedMaterial = materials[selectedMaterialIndex]; // Save it as fallback
//                Debug.Log("Applied default material from index.");
//            }
//        }
//    }


//    // Call this method when a button is pressed



//    public void Redtrigger()
//    {
//        isDecalRed = true;
//        isDecalGreen = false;
//        isDecalBlue = false;
//        isDecalYellow = false;

//        Debug.Log("Red True");

//    }

//    public void Greentrigger()
//    {
//        isDecalRed = false;
//        isDecalGreen = true;
//        isDecalBlue = false;
//        isDecalYellow = false;

//        Debug.Log("Green True");

//    }

//    public void Bluetrigger()
//    {
//        isDecalRed = false;
//        isDecalGreen = false;
//        isDecalBlue = true;
//        isDecalYellow = false;


//        Debug.Log("Blue True");

//    }

//    public void Yellowtrigger()
//    {
//        isDecalRed = false;
//        isDecalGreen = false;
//        isDecalBlue = false;
//        isDecalYellow = true;


//        Debug.Log("Blue True");

//    }

//    public void SetRedODecals()
//    {
//        if (!isDecalRed) return;

//        if (targetRenderer != null && redDecalOMaterial != null)
//        {
//            targetRenderer.material = redDecalOMaterial;
//            selectedODecal = redDecalOMaterial;
//            selectedMaterial = redDecalOMaterial; // ? Save the applied material
//            Debug.Log("Red O Decal Material applied.");
//        }
//        else
//        {
//            Debug.LogWarning("Renderer or redDecalOMaterial is missing!");
//        }
//    }

//    public void SetRedSDecals()
//    {
//        if (!isDecalRed) return;
//        if (targetRenderer != null && redDecalSMaterial != null)
//        {
//            targetRenderer.material = redDecalSMaterial;
//            selectedSDecal = redDecalSMaterial;
//            selectedMaterial = redDecalSMaterial; // ? Save the applied material
//            Debug.Log("Red S Decal Material applied.");
//        }
//        else
//        {
//            Debug.LogWarning("Renderer or redDecalSMaterial is missing!");
//        }
//    }

//    public void SetGreenODecals()
//    {
//        if (!isDecalGreen) return;
//        if (targetRenderer != null && greenDecalOMaterial != null)
//        {
//            targetRenderer.material = greenDecalOMaterial;
//            selectedODecal = greenDecalOMaterial;
//            selectedMaterial = greenDecalOMaterial; // ? Save the applied material
//            Debug.Log("Green O Decal Material applied.");
//        }
//        else
//        {
//            Debug.LogWarning("Renderer or greenDecalOMaterial is missing!");
//        }
//    }

//    public void SetGreenSDecals()
//    {
//        if (!isDecalGreen) return;
//        if (targetRenderer != null && greenDecalSMaterial != null)
//        {
//            targetRenderer.material = greenDecalSMaterial;
//            selectedSDecal = greenDecalSMaterial;
//            selectedMaterial = greenDecalSMaterial; // ? Save the applied material
//            Debug.Log("Green S Decal Material applied.");
//        }
//        else
//        {
//            Debug.LogWarning("Renderer or greenDecalSMaterial is missing!");
//        }
//    }

//    public void SetBlueODecals()
//    {
//        if (!isDecalBlue) return;
//        if (targetRenderer != null && blueDecalOMaterial != null)
//        {
//            targetRenderer.material = blueDecalOMaterial;
//            selectedODecal = blueDecalOMaterial;
//            selectedMaterial = blueDecalOMaterial; // ? Save the applied material
//            Debug.Log("Blue O Decal Material applied.");
//        }
//        else
//        {
//            Debug.LogWarning("Renderer or blueDecalOMaterial is missing!");
//        }
//    }

//    public void SetBlueSDecals()
//    {
//        if (!isDecalBlue) return;
//        if (targetRenderer != null && blueDecalSMaterial != null)
//        {
//            targetRenderer.material = blueDecalSMaterial;
//            selectedSDecal = blueDecalSMaterial;
//            selectedMaterial = blueDecalSMaterial; // ? Save the applied material
//            Debug.Log("Blue S Decal Material applied.");
//        }
//        else
//        {
//            Debug.LogWarning("Renderer or blueDecalSMaterial is missing!");
//        }
//    }

//    public void SetYellowODecals()
//    {
//        if (!isDecalYellow) return;
//        if (targetRenderer != null && yellowDecalOMaterial != null)
//        {
//            targetRenderer.material = yellowDecalOMaterial;
//            selectedODecal = yellowDecalOMaterial;
//            selectedMaterial = yellowDecalOMaterial; // ? Save the applied material
//            Debug.Log("Yellow O Decal Material applied.");
//        }
//        else
//        {
//            Debug.LogWarning("Renderer or yellowDecalOMaterial is missing!");
//        }
//    }


//    public void SetYellowSDecals()
//    {
//        if (!isDecalYellow) return;
//        if (targetRenderer != null && yellowDecalSMaterial != null)
//        {
//            targetRenderer.material = yellowDecalSMaterial;
//            selectedSDecal = yellowDecalSMaterial;
//            selectedMaterial = yellowDecalSMaterial; // ? Save the applied material
//            Debug.Log("Yellow S Decal Material applied.");
//        }
//        else
//        {
//            Debug.LogWarning("Renderer or yellowDecalSMaterial is missing!");
//        }
//    }





//    public void SwitchMaterial(int materialIndex)
//    {
//        if (materials != null && materialIndex >= 0 && materialIndex < materials.Length)
//        {
//            selectedMaterialIndex = materialIndex;
//            selectedMaterial = materials[materialIndex]; // ? Save the selected material
//            targetRenderer.material = selectedMaterial;
//        }
//        else
//        {
//            Debug.LogWarning("Invalid material index or materials array is empty!");
//        }
//    }
//}

