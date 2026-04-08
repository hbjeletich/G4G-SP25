using System;
using System.Collections.Generic;
using UnityEngine;

namespace CameraSnap
{
    public class CameraPan : MonoBehaviour
    {
        public float maxYaw = 60f;
        public float panSpeed = 30f;
        public float deadzone = 0.05f;
        public float decelerationSpeed = 5f;
        public float lockOnSpeed = 5f;

        [Header("FOV")]
        public float defaultFOV = 60f;
        public float zoomedFOV = 25f;
        public float totalZoomDuration = 5f; 
        private bool isZooming = false;
        private float zoomTimer = 0f;

        private float currentYaw = 0f;
        private float currentInput = 0f;

        private Transform lockTarget;
        private List<Transform> completedTargets = new List<Transform>();
        private Camera mainCamera;

        void Start()
        {
            mainCamera = Camera.main;
            mainCamera.fieldOfView = defaultFOV;
        }

        void LateUpdate()
        {
            if (lockTarget != null)
            {
                UIManager.Instance?.ShowStarProgressBar();
                // Calculate the target yaw relative to our parent (the cart/player)
                Vector3 direction = lockTarget.position - transform.parent.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                {
                    // Get the target yaw in parent's local space
                    float worldYaw = Quaternion.LookRotation(direction).eulerAngles.y;
                    float parentYaw = transform.parent.eulerAngles.y;
                    float targetYaw = Mathf.DeltaAngle(parentYaw, worldYaw);
                    targetYaw = Mathf.Clamp(targetYaw, -maxYaw, maxYaw);

                    currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, lockOnSpeed * Time.deltaTime);
                }

                transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);

                if (isZooming)
                {
                    zoomTimer += Time.deltaTime;
                    float t = Mathf.Clamp01(zoomTimer / totalZoomDuration);
                    mainCamera.fieldOfView = Mathf.Lerp(defaultFOV, zoomedFOV, t);
                    UIManager.Instance?.SetStarProgress(t);
                }
            } 
            else
            {
                Debug.Log($"CURRENT INPUT: {currentInput}");
                if (Mathf.Abs(currentInput) <= deadzone)
                {
                    currentInput = Mathf.Lerp(currentInput, 0f, decelerationSpeed * Time.deltaTime);
                }

                currentYaw += currentInput * panSpeed * Time.deltaTime;
                currentYaw = Mathf.Clamp(currentYaw, -maxYaw, maxYaw);

                transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
            }

            if(!isZooming && mainCamera.fieldOfView != defaultFOV)
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFOV, decelerationSpeed * Time.deltaTime);
                UIManager.Instance?.HideStarProgressBar();
            }
        }

        public void ManualPan(float weightShiftX)
        {
            currentInput = weightShiftX;
        }

        public void SetLockTarget(Transform target)
        {
            if(target == null || completedTargets.Contains(target)) return;
            lockTarget = target.transform;
        }

        public void ClearLockTarget()
        {
            completedTargets.Add(lockTarget);
            lockTarget = null;
            UIManager.Instance?.HideStarProgressBar();
            UIManager.Instance?.HideHoldText();
        }   

        public void DoZoom()
        {
            isZooming = true;
            zoomTimer = 0f;
            UIManager.Instance?.ShowHoldText();
        }

        public void StopZoom()
        {
            isZooming = false;
            UIManager.Instance?.HideHoldText();
        }
    }
}