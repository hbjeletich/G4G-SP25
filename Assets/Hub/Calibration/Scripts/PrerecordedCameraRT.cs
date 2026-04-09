using UnityEngine;

public class PrerecordedCameraRT : MonoBehaviour
{
    [Header("Render Texture")]
    public RenderTexture renderTexture;

    [Header("Camera Framing")]
    public float cameraDistance = 3.0f;
    public float cameraHeight = 1.2f;
    public float fieldOfView = 50f;
    public float lookAtHeight = 1.0f;

    [Tooltip("World-space direction FROM the camera TOWARD the character.")]
    public Vector3 cameraForward = Vector3.forward;

    // TODO: fix FBX root bone offset in Blender, then remove this workaround
    [Header("FBX Offset Workaround")]
    [Tooltip("Added to hip Y to compensate for broken root bone offset in the FBX. " +
             "Set to ~13 if your skeleton drops to Y=-13.")]
    public float yOffset = 0f;

    private Camera cam;
    private Transform hips;
    private Vector3 initialHipsXZ;
    private bool initialized = false;
    private bool isFullWidth = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = fieldOfView;
            if (renderTexture != null)
                cam.targetTexture = renderTexture;

            // Don't render until the flow controller enables us
            cam.enabled = false;
        }

        hips = transform.parent;
    }

    private void LateUpdate()
    {
        if (hips == null) return;

        // Capture initial X/Z on first valid frame
        if (!initialized)
        {
            initialHipsXZ = hips.position;
            initialized = true;
        }

        // Lock the skeleton's X/Z to prevent root motion drift
        Vector3 lockedPos = hips.position;
        lockedPos.x = initialHipsXZ.x;
        lockedPos.z = initialHipsXZ.z;
        hips.position = lockedPos;

        float scale = hips.lossyScale.y;
        Vector3 forward = cameraForward.normalized;

        // Lock X/Z to initial position, only Y follows the hips
        Vector3 anchorPos = new Vector3(
            initialHipsXZ.x,
            hips.position.y + (yOffset * scale),
            initialHipsXZ.z
        );

        Vector3 target = anchorPos;
        target.y = anchorPos.y + (lookAtHeight * scale);

        Vector3 camPos = target - forward * (cameraDistance * scale);
        camPos.y = anchorPos.y + (cameraHeight * scale);

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
        if (renderTexture == null || cam == null) return;
        int w = isFullWidth ? Screen.width : Screen.width / 2;
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