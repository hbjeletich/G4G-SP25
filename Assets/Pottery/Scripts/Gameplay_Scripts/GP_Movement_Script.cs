using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GP_Movement_Script : MonoBehaviour
{
    public float Progress = 0;
    public float Max = 40;
    public float progressSpeed = 1f;

    public Slider slider;
    public int slidervalue = 0;

    private MeshFilter meshFilter;

    public Mesh mesh01;
    public Mesh mesh02;
    public Mesh mesh03;
    public Mesh mesh04;
    public Mesh mesh05;
    public Mesh mesh06;

    public InputActionAsset inputActionAsset;
    private InputAction leftHipAbducted;
    private InputAction rightHipAbducted;

    private Coroutine progressRoutine;

    void Awake()
    {
        // ❗ You forgot this
        meshFilter = GetComponent<MeshFilter>();

        var footMap = inputActionAsset.FindActionMap("Foot");
        leftHipAbducted = footMap.FindAction("LeftHipAbducted");
        rightHipAbducted = footMap.FindAction("RightHipAbducted");
    }

    void OnEnable()
    {
        leftHipAbducted.Enable();
        leftHipAbducted.started += OnLeftHipStarted;
        leftHipAbducted.canceled += OnLeftHipCanceled;

        rightHipAbducted.Enable();
        rightHipAbducted.started += OnRightHipStarted;
        rightHipAbducted.canceled += OnRightHipCanceled;
    }

    void OnDisable()
    {
        leftHipAbducted.Disable();
        leftHipAbducted.started -= OnLeftHipStarted;
        leftHipAbducted.canceled -= OnLeftHipCanceled;

        rightHipAbducted.Disable();
        rightHipAbducted.started -= OnRightHipStarted;
        rightHipAbducted.canceled -= OnRightHipCanceled;
    }

    void OnLeftHipStarted(InputAction.CallbackContext context)
    {
        if (progressRoutine == null)
            progressRoutine = StartCoroutine(AddProgressWhileActive());
    }

    void OnLeftHipCanceled(InputAction.CallbackContext context)
    {
        if (progressRoutine != null)
        {
            StopCoroutine(progressRoutine);
            progressRoutine = null;
        }
    }

    void OnRightHipStarted(InputAction.CallbackContext context) { }
    void OnRightHipCanceled(InputAction.CallbackContext context) { }


    // ------------------------------------------
    // Update mesh based on slider value
    // ------------------------------------------
    void UpdateMesh()
    {
        switch (slidervalue)
        {
            case 0:  meshFilter.mesh = mesh01; break;
            case 7:  meshFilter.mesh = mesh02; break;
            case 14: meshFilter.mesh = mesh03; break;
            case 21: meshFilter.mesh = mesh04; break;
            case 35: meshFilter.mesh = mesh05; break;
            case 40: meshFilter.mesh = mesh06; break;
        }
    }

    public void ResetProgress()
    {
        Progress = 0;
        slider.value = 0;
        slidervalue = 0;
        UpdateMesh();   // Returns to mesh01
    }

    // ------------------------------------------
    // Main coroutine
    // ------------------------------------------
    IEnumerator AddProgressWhileActive()
    {
        while (leftHipAbducted.ReadValue<float>() > 0)
        {
            Progress += progressSpeed * Time.deltaTime;
            slider.value = Progress;

            // ❗ Update slidervalue
            slidervalue = Mathf.RoundToInt(Progress);

            // ❗ Update mesh LIVE
            UpdateMesh();

            yield return null;
        }

        progressRoutine = null;
    }
}
