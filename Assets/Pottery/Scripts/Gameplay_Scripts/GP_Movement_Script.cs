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


    void Start()
    {
        UpdateMesh();
    }

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
        int v = slidervalue;

        if (v >= 0 && v < 7)
            meshFilter.mesh = mesh01;
        else if (v >= 7 && v < 14)
            meshFilter.mesh = mesh02;
        else if (v >= 14 && v < 21)
            meshFilter.mesh = mesh03;
        else if (v >= 21 && v < 35)
            meshFilter.mesh = mesh04;
        else if (v >= 35 && v < 40)
            meshFilter.mesh = mesh05;
        else if (v >= 40)
            meshFilter.mesh = mesh06;
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

    public void ResetProgress()
    {
        StartCoroutine(ResetAfterFrames());
    }

    IEnumerator ResetAfterFrames()
    {
        // Wait 5 frames
        for (int i = 0; i < 5; i++)
            yield return null;

        // Stop any running progress coroutine
        if (progressRoutine != null)
        {
            StopCoroutine(progressRoutine);
            progressRoutine = null;
        }

        // Reset values
        Progress = 0;
        slider.value = 0;
        slidervalue = 0;

        UpdateMesh();
    }
}
