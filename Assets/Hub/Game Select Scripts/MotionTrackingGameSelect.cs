using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MotionTrackingGameSelect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameCarousel gameCarousel;

    [Header("Weight Shift Settings")]
    [SerializeField] private float weightShiftThreshold = 0.5f;
    [SerializeField] private float navigationCooldown = 0.5f;

    [Header("Squat Settings")]
    [SerializeField] private float squatThreshold = 0.6f;
    [SerializeField] private float squatCooldown = 1.0f;

    private InputAction weightShiftXAction;
    private InputAction squatTrackingYAction;

    private float lastNavigationTime;
    private float lastSquatTime;
    private bool isSquatting = false;

    private void Awake()
    {
        var torsoMap = inputActions.FindActionMap("Torso");
        weightShiftXAction = torsoMap.FindAction("WeightShiftX");
        squatTrackingYAction = torsoMap.FindAction("PelvisPosition");
    }

    private void OnEnable()
    {
        weightShiftXAction.Enable();
        squatTrackingYAction.Enable();
    }

    private void OnDisable()
    {
        weightShiftXAction.Disable();
        squatTrackingYAction.Disable();
    }

    private void Update()
    {
        float weightShift = weightShiftXAction.ReadValue<float>();
        HandleNavigation(weightShift);

        float squatValue = squatTrackingYAction.ReadValue<Vector3>().y;
        HandleSquatSelection(squatValue);
    }

    private void HandleNavigation(float input)
    {
        if (Time.time - lastNavigationTime < navigationCooldown) return;

        if (input > weightShiftThreshold)
        {
            gameCarousel.GoToNext();
            lastNavigationTime = Time.time;
        }
        else if (input < -weightShiftThreshold)
        {
            gameCarousel.GoToPrevious();
            lastNavigationTime = Time.time;
        }
    }

    private void HandleSquatSelection(float squatValue)
    {
        if (Time.time - lastSquatTime < squatCooldown) return;

        if (squatValue < squatThreshold && !isSquatting)
        {
            isSquatting = true;
            lastSquatTime = Time.time;
            gameCarousel.PlayCurrentGame();
        }
        else if (squatValue > squatThreshold)
        {
            isSquatting = false;
        }
    }
}