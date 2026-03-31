using UnityEngine;

// RIGHT camera — parented under the prerecorded skeleton's hips.
// Follows hip position, ignores hip rotation.
public class PrerecordedCameraRT : MonoBehaviour
{
    [Header("Render Texture")]
    public RenderTexture renderTexture;

    [Header("Camera Framing")]
    public float cameraDistance = 3.0f;
    public float cameraHeight = 1.2f;
    public float fieldOfView = 50f;
    public float lookAtHeight = 1.0f;
    public bool flipForward = true;

    private Camera cam;
    private Transform hips;
    private Vector3 fixedForward;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = fieldOfView;
            if (renderTexture != null)
                cam.targetTexture = renderTexture;
        }

        hips = transform.parent;

        Transform root = FindAnimatorRoot();
        if (root != null)
            fixedForward = flipForward ? -root.forward : root.forward;
        else
            fixedForward = flipForward ? -Vector3.forward : Vector3.forward;
    }

    private void LateUpdate()
    {
        if (hips == null) return;

        float scale = hips.lossyScale.y;

        Vector3 hipsPos = hips.position;
        Vector3 target = hipsPos;
        target.y = hipsPos.y + (lookAtHeight * scale);

        Vector3 camPos = target - fixedForward * (cameraDistance * scale);
        camPos.y = hipsPos.y + (cameraHeight * scale);

        transform.position = camPos;
        transform.LookAt(target);

        ResizeRT();
    }

    private Transform FindAnimatorRoot()
    {
        Transform current = transform.parent;
        while (current != null)
        {
            if (current.GetComponent<Animator>() != null)
                return current;
            current = current.parent;
        }
        return transform.parent;
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