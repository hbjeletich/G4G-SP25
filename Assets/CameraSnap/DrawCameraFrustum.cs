using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCameraFrustum : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        Gizmos.color = Color.red;

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = mainCamera.transform.localToWorldMatrix;

        Gizmos.DrawFrustum(
            Vector3.zero,
            mainCamera.fieldOfView,
            mainCamera.farClipPlane,
            mainCamera.nearClipPlane,
            mainCamera.aspect
        );

        Gizmos.matrix = oldMatrix;
    }
}
