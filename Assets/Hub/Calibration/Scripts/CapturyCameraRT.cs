using UnityEngine;
using Captury;

public class CapturyCameraRT : MonoBehaviour
{
    [Header("Render Texture")]
    public RenderTexture renderTexture;
    public RenderTexture fullWidthRenderTexture;
    private RenderTexture activeRenderTexture;

    [Header("Camera Framing")]
    public string hipJointName = "Hips";
    public float cameraDistance = 3.0f;
    public float cameraHeight = 1.2f;
    public float fieldOfView = 50f;
    public float lookAtHeight = 1.0f;

    [Tooltip("World-space direction FROM the camera TOWARD the character.")]
    public Vector3 cameraForward = Vector3.forward;

    [Header("Drift Correction")]
    [Tooltip("How quickly the anchor drifts back to the initial position. 0 = locked, 1 = no correction.")]
    [Range(0f, 1f)]
    public float driftCorrectionStrength = 0.05f;

    private Camera cam;
    private CapturyNetworkPlugin networkPlugin;
    private Transform hipBone;
    private Vector3 anchorXZ;
    private bool ready = false;
    private bool initialized = false;
    private bool isFullWidth = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = fieldOfView;
            // if (renderTexture != null)
            //     cam.targetTexture = renderTexture;
            if(fullWidthRenderTexture != null)
                cam.targetTexture = fullWidthRenderTexture;
        }

        networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound += OnSkeletonFound;
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

    private void OnSkeletonFound(CapturySkeleton skeleton)
    {
        skeleton.OnSkeletonSetupComplete += OnSkeletonReady;
    }

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
            Debug.LogError($"CapturyCameraRT: Could not find joint '{hipJointName}'");
            return;
        }

        ready = true;
        initialized = false; // re-capture anchor for new skeleton
        Debug.Log($"CapturyCameraRT: Ready — tracking '{hipJointName}'");
    }

    private void LateUpdate()
    {
        if (!ready || hipBone == null) return;

        // Capture initial X/Z on first valid frame
        if (!initialized)
        {
            anchorXZ = hipBone.position;
            initialized = true;
        }

        // Soft drift correction: slowly pull anchor back toward initial position
        // This absorbs accumulated mocap noise while allowing real movement to show briefly
        Vector3 currentHipsXZ = new Vector3(hipBone.position.x, 0, hipBone.position.z);
        Vector3 initialXZ = new Vector3(anchorXZ.x, 0, anchorXZ.z);
        Vector3 correctedXZ = Vector3.Lerp(currentHipsXZ, initialXZ, driftCorrectionStrength);

        Vector3 forward = cameraForward.normalized;

        Vector3 anchorPos = new Vector3(
            correctedXZ.x,
            hipBone.position.y,
            correctedXZ.z
        );

        Vector3 target = anchorPos;
        target.y = anchorPos.y + lookAtHeight;

        Vector3 camPos = target - forward * cameraDistance;
        camPos.y = anchorPos.y + cameraHeight;

        transform.position = camPos;
        transform.LookAt(target);

        ResizeRT();
    }

    public void SetFullWidth(bool fullWidth)
    {
        isFullWidth = fullWidth;
    }

    private void ResizeRT()
    {
        // if (activeRenderTexture == null || cam == null) return;
        // int w = isFullWidth ? Screen.width : Screen.width / 2;
        // int h = Screen.height;
        // if (w < 1 || h < 1) return;
        // if (activeRenderTexture.width != w || activeRenderTexture.height != h)
        // {
        //     cam.targetTexture = null;
        //     activeRenderTexture.Release();
        //     activeRenderTexture.width = w;
        //     activeRenderTexture.height = h;
        //     cam.targetTexture = activeRenderTexture;
        // }
    }

    public void SwapRenderTexture(bool fullWidth)
    {
        if (cam == null) return;

        if (fullWidth)
        {
            activeRenderTexture = fullWidthRenderTexture;
        }
        else
        {
            activeRenderTexture = renderTexture;
        }

        cam.targetTexture = activeRenderTexture;
    }
}