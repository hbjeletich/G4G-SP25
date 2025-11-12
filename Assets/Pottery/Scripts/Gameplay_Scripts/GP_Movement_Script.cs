using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GP_Movement_Script : MonoBehaviour
{
    public float Progress = 0;
    public float Max = 40;
    public float progressSpeed = 1f; // how fast Progress increases per second
    private bool nextStage = false;

    public Slider slider;

    public InputActionAsset inputActionAsset;
    private InputAction leftHipAbducted;
    private InputAction rightHipAbducted;

    private Coroutine progressRoutine;

    void Awake()
    {
        var footMap = inputActionAsset.FindActionMap("Foot");
        if (footMap == null)
        {
            Debug.LogWarning("Pottery: Foot map not found!");
            return;
        }
        Debug.Log("Pottery: Foot map found!");


        leftHipAbducted = footMap.FindAction("LeftHipAbducted");
        if (leftHipAbducted == null)
            Debug.LogWarning("Pottery: Left Hip Abducted not found!");
        else
            Debug.Log("Pottery: Left Hip Abducted found!");


        rightHipAbducted = footMap.FindAction("RightHipAbducted");
        if (rightHipAbducted == null)
            Debug.LogWarning("Pottery: right Hip Abducted not found!");
        else
            Debug.Log("Pottery: right Hip Abducted found!");


    }

    void OnEnable()
    {
        if (leftHipAbducted == null) return;

        leftHipAbducted.Enable();
        leftHipAbducted.started += OnLeftHipStarted;
        leftHipAbducted.canceled += OnLeftHipCanceled;

        Debug.Log("Pottery: OnEnable");


        if (rightHipAbducted == null) return;

        rightHipAbducted.Enable();
        rightHipAbducted.started += OnLeftHipStarted;
        rightHipAbducted.canceled += OnLeftHipCanceled;

    }

    void OnDisable()
    {
        if (leftHipAbducted == null) return;

        leftHipAbducted.Disable();
        leftHipAbducted.started -= OnLeftHipStarted;
        leftHipAbducted.canceled -= OnLeftHipCanceled;

        Debug.Log("Pottery: OnDisable");

        if (rightHipAbducted == null) return;

        rightHipAbducted.Disable();
        rightHipAbducted.started -= OnRightHipStarted;
        rightHipAbducted.canceled -= OnRightHipCanceled;
    }

    void OnLeftHipStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Pottery: Left Hip started (active)");
        if (progressRoutine == null)
            progressRoutine = StartCoroutine(AddProgressWhileActive());
    }

    void OnLeftHipCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Pottery: Left Hip canceled (released)");
        if (progressRoutine != null)
        {
            StopCoroutine(progressRoutine);
            progressRoutine = null;
        }
    }

    void OnRightHipStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Pottery: right Hip started (active)");

    }


    void OnRightHipCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Pottery: right Hip started (active)");

    }




    IEnumerator AddProgressWhileActive()
    {
        while (leftHipAbducted.ReadValue<float>() > 0) // assumes float control (e.g., analog)
        {
            Progress += progressSpeed * Time.deltaTime;
            slider.value = Progress;

            if (Progress >= Max && !nextStage)
            {
                nextStage = true;
                Debug.Log("Slider Full, nextStage = true");
            }

            yield return null;
        }

        progressRoutine = null;
    }
}
