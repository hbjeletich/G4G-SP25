using UnityEngine;
using UnityEditor;

namespace Fishing
{
[InitializeOnLoad]
public class CameraBoundsDrawer
{
    static CameraBoundsDrawer()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Camera cam = Camera.main; // Get the main camera
        if (cam == null || !cam.orthographic) return;

        Handles.color = Color.green; // Set outline color
        float height = cam.orthographicSize * 2;
        float width = height * cam.aspect;
        Vector3 center = cam.transform.position;

        Vector3[] corners = {
            center + new Vector3(-width / 2, height / 2, 0),
            center + new Vector3(width / 2, height / 2, 0),
            center + new Vector3(width / 2, -height / 2, 0),
            center + new Vector3(-width / 2, -height / 2, 0)
        };

        Handles.DrawLine(corners[0], corners[1]);
        Handles.DrawLine(corners[1], corners[2]);
        Handles.DrawLine(corners[2], corners[3]);
        Handles.DrawLine(corners[3], corners[0]);

        SceneView.RepaintAll(); // Refresh the Scene view
    }
}
}
