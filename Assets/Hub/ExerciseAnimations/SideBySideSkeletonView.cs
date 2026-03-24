using UnityEngine;

public class SideBySideSkeletonView : MonoBehaviour
{
    [Header("References")]
    public Transform prerecordedAnchor;
    public string capturySkeletonNameContains = "";

    [Header("Layout")]
    public float sideOffset = 1.0f;
    public float prerecordedY = 0f;
    public float prerecordedForwardOffset = 0f;

    [Header("Camera Framing")]
    public float cameraDistance = 3.0f;
    public float cameraHeight = 1.2f;
    public float fieldOfView = 50f;

    private Transform capturySkeleton;
    private Camera cam;
    private bool foundSkeleton = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
            cam.fieldOfView = fieldOfView;

        if (prerecordedAnchor != null)
            prerecordedAnchor.localPosition = new Vector3(sideOffset, prerecordedY, prerecordedForwardOffset);
    }

    private void LateUpdate()
    {
        if (!foundSkeleton)
        {
            capturySkeleton = FindCapturySkeleton();
            if (capturySkeleton != null)
            {
                foundSkeleton = true;
                Debug.Log($"SideBySideView: Found Captury skeleton '{capturySkeleton.name}'");
            }
        }

        if (foundSkeleton && capturySkeleton == null)
        {
            foundSkeleton = false;
            return;
        }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 target;

        if (capturySkeleton != null && prerecordedAnchor != null)
        {
            target = (capturySkeleton.position + prerecordedAnchor.position) * 0.5f;
        }
        else if (prerecordedAnchor != null)
        {
            target = prerecordedAnchor.position;
        }
        else
        {
            return;
        }

        target.y = transform.parent.position.y + cameraHeight;

        Vector3 camPos = target - transform.parent.forward * cameraDistance;
        camPos.y = target.y;

        transform.position = camPos;
        transform.LookAt(target);
    }

    private Transform FindCapturySkeleton()
    {
        if (transform.parent == null) return null;

        Transform trackingArea = transform.parent;

        for (int i = 0; i < trackingArea.childCount; i++)
        {
            Transform child = trackingArea.GetChild(i);

            if (child == transform) continue;
            if (prerecordedAnchor != null && child == prerecordedAnchor) continue;

            if (!string.IsNullOrEmpty(capturySkeletonNameContains))
            {
                if (child.name.Contains(capturySkeletonNameContains))
                    return child;
            }
            else
            {
                if (child.GetComponent<Animator>() != null || child.childCount > 5)
                    return child;
            }
        }

        return null;
    }
}