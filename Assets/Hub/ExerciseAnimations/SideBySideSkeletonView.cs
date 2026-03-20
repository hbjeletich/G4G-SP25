using UnityEngine;

public class SideBySideSkeletonView : MonoBehaviour
{
    [Header("References")]
    public Transform prerecordedAnchor;
    public string capturySkeletonNameContains = "";

    [Header("Layout")]
    public float sideOffset = 1.0f;
    public float prerecordedForwardOffset = 0f;

    [Header("Auto-Follow")]
    public bool autoFollowLiveSkeleton = false;
    [Range(1f, 60f)]
    public float followUpdatesPerSecond = 10f;
    [Range(0f, 0.95f)]
    public float followSmoothing = 0.5f;

    [Header("Camera Framing")]
    public float cameraDistance = 3.0f;
    public float cameraHeight = 1.2f;
    public float fieldOfView = 50f;

    private Transform capturySkeleton;
    private Camera cam;
    private bool foundSkeleton = false;
    private float followTimer = 0f;
    private Vector3 followTargetPos;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = fieldOfView;
        }

        // position prerecorded to the right
        if (prerecordedAnchor != null)
        {
            prerecordedAnchor.localPosition = new Vector3(sideOffset, 0f, prerecordedForwardOffset);
            followTargetPos = prerecordedAnchor.position;
        }
    }

    private void LateUpdate()
    {
        // looking for the Captury skeleton
        if (!foundSkeleton)
        {
            capturySkeleton = FindCapturySkeleton();
            if (capturySkeleton != null)
            {
                foundSkeleton = true;
                Debug.Log($"SideBySideView: Found Captury skeleton '{capturySkeleton.name}'");
            }
        }

        // if skeleton was destroyed (actor left), look again next frame
        if (foundSkeleton && capturySkeleton == null)
        {
            foundSkeleton = false;
            return;
        }

        UpdateAutoFollow();
        UpdateCameraPosition();
    }

    private void UpdateAutoFollow()
    {
        if (!autoFollowLiveSkeleton || capturySkeleton == null || prerecordedAnchor == null)
            return;

        followTimer += Time.deltaTime;
        float interval = 1f / followUpdatesPerSecond;

        if (followTimer >= interval)
        {
            followTimer -= interval;

            // where prerecorded should be relative to live skeleton
            Vector3 livePos = capturySkeleton.position;
            followTargetPos = new Vector3(
                livePos.x + sideOffset * 2f,
                livePos.y,
                livePos.z + prerecordedForwardOffset
            );
        }

        // move toward the target every frame for visual smoothness
        if (followSmoothing > 0f)
        {
            prerecordedAnchor.position = Vector3.Lerp(
                prerecordedAnchor.position,
                followTargetPos,
                1f - Mathf.Pow(followSmoothing, Time.deltaTime * 60f)
            );
        }
        else
        {
            prerecordedAnchor.position = followTargetPos;
        }
    }

    private void UpdateCameraPosition()
    {
        if (capturySkeleton == null)
        {
            // no live skeleton yet — just frame the prerecorded model
            if (prerecordedAnchor != null)
            {
                Vector3 target = prerecordedAnchor.position;
                target.y += cameraHeight;
                transform.position = target - transform.parent.forward * cameraDistance;
                transform.LookAt(target);
            }
            return;
        }

        // midpoint between live skeleton and prerecorded
        Vector3 livePos = capturySkeleton.position;
        Vector3 prerecordedPos = prerecordedAnchor != null
            ? prerecordedAnchor.position
            : livePos + Vector3.right * sideOffset * 2f;

        Vector3 midpoint = (livePos + prerecordedPos) * 0.5f;
        midpoint.y = transform.parent.position.y + cameraHeight;

        // position camera looking at the midpoint from the front
        Vector3 camPos = midpoint - transform.parent.forward * cameraDistance;
        camPos.y = transform.parent.position.y + cameraHeight;

        transform.position = camPos;
        transform.LookAt(midpoint);
    }

    private Transform FindCapturySkeleton()
    {
        if (transform.parent == null) return null;

        // search siblings
        Transform trackingArea = transform.parent;

        for (int i = 0; i < trackingArea.childCount; i++)
        {
            Transform child = trackingArea.GetChild(i);

            // skip ourselves and the prerecorded anchor
            if (child == transform) continue;
            if (prerecordedAnchor != null && child == prerecordedAnchor) continue;

            // by name if specified
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

    public void MatchPrerecordedToLiveSkeleton()
    {
        if (capturySkeleton == null || prerecordedAnchor == null) return;

        Vector3 livePos = capturySkeleton.position;
        prerecordedAnchor.position = new Vector3(
            livePos.x + sideOffset * 2f,
            livePos.y,
            livePos.z + prerecordedForwardOffset
        );
    }
}