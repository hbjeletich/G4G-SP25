using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MeshSwitcher4 : MonoBehaviour
{
    
    Settings_Script settingsscript;

    [Header("Assign your 4 meshes here")]
    public Mesh mesh01;
    public Mesh mesh02;
    public Mesh mesh03;
    public Mesh mesh04;

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

        //Press Right Arrow to advance
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchToNextMesh();
        }
    }

    void SwitchToNextMesh()
    {
        if (meshFilter == null) return;

        currentMeshIndex++;

        //if (settingsscript.progress == 0)
        //{
        //    meshFilter.mesh = mesh01;
        //}
        //else if (settingsscript.progress == 3)
        //{
        //    meshFilter.mesh = mesh02;
        //}
        //else if (settingsscript.progress == 6)
        //{
        //    meshFilter.mesh = mesh03;
        //}
        //else if (settingsscript.progress == 10)
        //{
        //    meshFilter.mesh = mesh04;
        //}

        switch (currentMeshIndex)
        {
            case 1:
                meshFilter.mesh = mesh02;
                break;
            case 2:
                meshFilter.mesh = mesh03;
                break;
            case 3:
                meshFilter.mesh = mesh04;
                break;
            default:
                // Stop at mesh04 (no cycling)
                currentMeshIndex = 3;
                break;
        }
    }
}
