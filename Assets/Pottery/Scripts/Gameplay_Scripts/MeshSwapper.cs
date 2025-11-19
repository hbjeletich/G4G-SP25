using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSwapper : MonoBehaviour
{

    Settings_Script settingsscript;

    public int slidervalue = 0;

    [Header("Assign your 4 meshes here")]
    public Mesh mesh01;
    public Mesh mesh02;
    public Mesh mesh03;
    public Mesh mesh04;
    public Mesh mesh05;
    public Mesh mesh06;


    private MeshFilter meshFilter;
    private int currentMeshIndex = 0;


    void Awake()
    {
        GameObject go = GameObject.Find("GameView");
        if (go != null)
        {
            settingsscript = go.GetComponent<Settings_Script>();
            if (settingsscript == null)
            {
                Debug.LogError("Settings_Script not found on GameView.");
            }
        }
        else
        {
            Debug.LogError("GameObject named 'GameView' not found in scene.");
        }
    }

  void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("No MeshFilter found on this GameObject!");
            return;
        }
        // Start with mesh01
        meshFilter.mesh = mesh01;
    }


    void Update()
    {
        updateMesh();
    }

    void updateMesh()
    {

        //If key is pressed, increase slidervalue by 1
        if (Input.GetKeyDown(KeyCode.Space))
        {
            slidervalue = (slidervalue + 1);

            Debug.Log("space pressed");
        }

        Debug.Log("slidervalue: " + slidervalue);


        //when slidervalue = #, change mesh to mesh#
        switch (slidervalue)
        {
            case 0:
                meshFilter.mesh = mesh01;
                break;
            case 7:
                meshFilter.mesh = mesh02;
                break;
            case 14:
                meshFilter.mesh = mesh03;
                break;
            case 21:
                meshFilter.mesh = mesh04;
                break;
            case 35:
                meshFilter.mesh = mesh05;
                break;
            case 40:
                meshFilter.mesh = mesh06;
                break;
            default:
                Debug.LogWarning("Invalid slidervalue: " + slidervalue);
                break;
        }
    }
}
