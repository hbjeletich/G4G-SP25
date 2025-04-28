using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeDebugger : MonoBehaviour
{
    void Update()
    {
        var volumeManager = VolumeManager.instance;
        var layerMask = 1 << gameObject.layer; // Get current object's layer as a LayerMask

        var volumes = volumeManager.GetVolumes(layerMask);
        bool anyActive = false;

        foreach (var volume in volumes)
        {
            if (volume != null && volume.enabled && volume.isActiveAndEnabled)
            {
                Debug.Log($"[Volume Debug] ACTIVE: {volume.name}, Priority: {volume.priority}, Weight: {volume.weight}");
                anyActive = true;
            }
        }

        if (!anyActive)
        {
            Debug.Log("[Volume Debug] No active volumes found on current layer.");
        }

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Debug.Log("[Cam Debug] Main camera is: " + mainCam.name + ", Layer: " + LayerMask.LayerToName(mainCam.gameObject.layer));
        }
        else
        {
            Debug.Log("[Cam Debug] No camera tagged as MainCamera");
        }
    }
}

