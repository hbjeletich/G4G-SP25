using UnityEngine;

[ExecuteInEditMode]

public class OrthographicCameraFrustum : MonoBehaviour
{
    private Camera cam;

    void OnDrawGizmos()
    {
        cam = GetComponent<Camera>();

        if (cam != null && cam.orthographic)
        {
            // Draw the camera's frustum for orthographic view in the Scene view
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;

            // For orthographic cameras, the size of the near and far planes are determined by the camera's orthographic size and aspect ratio
            float nearPlaneHeight = cam.orthographicSize * 2f;
            float nearPlaneWidth = nearPlaneHeight * cam.aspect;

            // Set the far plane dimensions equal to near plane (since it's orthographic)
            float farPlaneHeight = nearPlaneHeight;
            float farPlaneWidth = nearPlaneWidth;

            Vector3 nearCenter = new Vector3(0, 0, cam.nearClipPlane);
            Vector3 farCenter = new Vector3(0, 0, cam.farClipPlane);

            // Near and far plane corners
            Vector3 nearTopLeft = new Vector3(-nearPlaneWidth / 2f, nearPlaneHeight / 2f, cam.nearClipPlane);
            Vector3 nearTopRight = new Vector3(nearPlaneWidth / 2f, nearPlaneHeight / 2f, cam.nearClipPlane);
            Vector3 nearBottomLeft = new Vector3(-nearPlaneWidth / 2f, -nearPlaneHeight / 2f, cam.nearClipPlane);
            Vector3 nearBottomRight = new Vector3(nearPlaneWidth / 2f, -nearPlaneHeight / 2f, cam.nearClipPlane);

            Vector3 farTopLeft = new Vector3(-farPlaneWidth / 2f, farPlaneHeight / 2f, cam.farClipPlane);
            Vector3 farTopRight = new Vector3(farPlaneWidth / 2f, farPlaneHeight / 2f, cam.farClipPlane);
            Vector3 farBottomLeft = new Vector3(-farPlaneWidth / 2f, -farPlaneHeight / 2f, cam.farClipPlane);
            Vector3 farBottomRight = new Vector3(farPlaneWidth / 2f, -farPlaneHeight / 2f, cam.farClipPlane);

            // Draw near and far plane rectangles
            Gizmos.DrawLine(nearTopLeft, nearTopRight);
            Gizmos.DrawLine(nearTopRight, nearBottomRight);
            Gizmos.DrawLine(nearBottomRight, nearBottomLeft);
            Gizmos.DrawLine(nearBottomLeft, nearTopLeft);

            Gizmos.DrawLine(farTopLeft, farTopRight);
            Gizmos.DrawLine(farTopRight, farBottomRight);
            Gizmos.DrawLine(farBottomRight, farBottomLeft);
            Gizmos.DrawLine(farBottomLeft, farTopLeft);

            // Draw lines from near to far plane corners
            Gizmos.DrawLine(nearTopLeft, farTopLeft);
            Gizmos.DrawLine(nearTopRight, farTopRight);
            Gizmos.DrawLine(nearBottomLeft, farBottomLeft);
            Gizmos.DrawLine(nearBottomRight, farBottomRight);
        }
    }
}