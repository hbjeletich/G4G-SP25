using UnityEngine;

public class MatchRTToScreen : MonoBehaviour
{
    public Camera skeletonCam;
    public RenderTexture rt;
    
    void Update()
    {
        int halfWidth = Screen.width / 2;
        int height = Screen.height;
        
        if (rt.width != halfWidth || rt.height != height)
        {
            skeletonCam.targetTexture = null;
            rt.Release();
            rt.width = halfWidth;
            rt.height = height;
            skeletonCam.targetTexture = rt;
        }
    }
}