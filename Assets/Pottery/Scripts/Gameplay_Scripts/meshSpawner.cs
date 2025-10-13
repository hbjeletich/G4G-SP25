using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSwitcher4 : MonoBehaviour
{
    [Header("Assign your 4 meshes here")]
    public Mesh mesh01;
    public Mesh mesh02;
    public Mesh mesh03;
    public Mesh mesh04;

    private MeshFilter meshFilter;
    private int currentMeshIndex = 0;

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
        // Press Right Arrow to advance
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchToNextMesh();
        }
    }

    void SwitchToNextMesh()
    {
        if (meshFilter == null) return;

        currentMeshIndex++;

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
