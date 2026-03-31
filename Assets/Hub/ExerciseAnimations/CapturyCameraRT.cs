using UnityEngine;
using Captury;

/// <summary>
/// LEFT camera — finds the Captury live skeleton the same way the toolkit does.
/// Place on a camera GameObject in the scene.
/// </summary>
public class CapturyCameraRT : MonoBehaviour
{
    [Header("Render Texture")]
    public RenderTexture renderTexture;

    [Header("Camera Framing")]
    public string hipJointName = "Hips";
    public float cameraDistance = 3.0f;
    public float cameraHeight = 1.2f;
    public float fieldOfView = 50f;
    public float lookAtHeight = 1.0f;
    public bool flipForward = true;

    private Camera cam;
    private CapturyNetworkPlugin networkPlugin;
    private Transform hipBone;
    private Transform skeletonRoot;
    private Vector3 fixedForward;
    private bool ready = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = fieldOfView;
            if (renderTexture != null)
                cam.targetTexture = renderTexture;
        }

        // Same pattern as MotionTrackingManager.FindNetworkPlugin()
        networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound += OnSkeletonFound;
            Debug.Log("CapturyCameraRT: Subscribed to SkeletonFound");
        }
        else
        {
            Debug.LogError("CapturyCameraRT: No CapturyNetworkPlugin found in scene!");
        }
    }

    private void OnDestroy()
    {
        if (networkPlugin != null)
            networkPlugin.SkeletonFound -= OnSkeletonFound;
    }

    // Same pattern as MotionTrackingManager.OnSkeletonFound()
    private void OnSkeletonFound(CapturySkeleton skeleton)
    {
        Debug.Log("CapturyCameraRT: Skeleton found, waiting for setup...");
        skeleton.OnSkeletonSetupComplete += OnSkeletonReady;
    }

    // Same pattern as MotionTrackingManager.OnSkeletonSetupComplete()
    private void OnSkeletonReady(CapturySkeleton skeleton)
{
    foreach (var joint in skeleton.joints)
    {
        if (joint.name == hipJointName)
        {
            hipBone = joint.transform;
            break;
        }
    }

    if (hipBone == null)
    {
        Debug.LogError($"CapturyCameraRT: Could not find joint '{hipJointName}' in skeleton!");
        return;
    }

    // Walk up from the hip to find the skeleton's root transform
    skeletonRoot = hipBone.root;
    fixedForward = flipForward ? -skeletonRoot.forward : skeletonRoot.forward;
    ready = true;

    Debug.Log($"CapturyCameraRT: Ready — tracking '{hipJointName}' on '{skeletonRoot.name}'");
}

    private void LateUpdate()
    {
        if (!ready || hipBone == null) return;

        Vector3 hipsPos = hipBone.position;

        Vector3 target = hipsPos;
        target.y = hipsPos.y + lookAtHeight;

        Vector3 camPos = target - fixedForward * cameraDistance;
        camPos.y = hipsPos.y + cameraHeight;

        transform.position = camPos;
        transform.LookAt(target);

        ResizeRT();
    }

    private void ResizeRT()
    {
        if (renderTexture == null || cam == null) return;
        int w = Screen.width / 2;
        int h = Screen.height;
        if (w < 1 || h < 1) return;
        if (renderTexture.width != w || renderTexture.height != h)
        {
            cam.targetTexture = null;
            renderTexture.Release();
            renderTexture.width = w;
            renderTexture.height = h;
            cam.targetTexture = renderTexture;
        }
    }
}