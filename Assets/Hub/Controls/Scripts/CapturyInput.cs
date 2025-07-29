using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering;

public struct CapturyInputState : IInputStateTypeInfo
{
    // defining all trackable data

    // format code must be exactly 4 characters
    public FourCC format => new FourCC('C', 'A', 'P', 'T'); // creates a four charr5acter code for captury input

    // defining controls as seen in MotionControls.cs

    // foot tracking
    [InputControl(layout = "Axis")]
    public float footHeight;

    [InputControl(layout = "Button")]
    public float footRaise;

    [InputControl(layout = "Button")]
    public float footLower;

    // weight shifting
    [InputControl(layout = "Axis")]
    public float weightShiftX; // lateral shift (left-right)

    [InputControl(layout = "Button")]
    public float weightShiftLeft;

    [InputControl(layout = "Button")]
    public float weightShiftRight;

    // 4 step quadrant info
    [InputControl(layout = "Button")]
    public float quadrantOne;

    [InputControl(layout = "Button")]
    public float quadrantTwo;

    [InputControl(layout = "Button")]
    public float quadrantThree;

    [InputControl(layout = "Button")]
    public float quadrantFour;

    [InputControl(layout = "Button")]
    public float allQuadrantsComplete;

    // hip abduction controls
    [InputControl(layout = "Button")]
    public float leftHipAbducted;

    [InputControl(layout = "Button")]
    public float rightHipAbducted;

    [InputControl(layout = "Axis")]
    public float abductionDistance;

    [InputControl(layout = "Axis")]
    public float leftFootHeight;

    [InputControl(layout = "Axis")]
    public float rightFootHeight;

    // squat controls
    [InputControl(layout = "Axis")]
    public float squatTrackingY;

    [InputControl(layout = "Button")]
    public float squatStarted;

    [InputControl(layout = "Button")]
    public float squatCompleted;
}

[InputControlLayout(stateType = typeof(CapturyInputState), displayName = "Captury Input")]
public class CapturyInput : InputDevice
{
    // creating input device

    // foot tracking
    [InputControl(layout = "Axis", displayName = "Foot Height", usage = "Primary")]
    public AxisControl footHeight { get; private set; }

    [InputControl(layout = "Button", displayName = "Foot Raise")]
    public ButtonControl footRaise { get; private set; }

    [InputControl(layout = "Button", displayName = "Foot Lower")]
    public ButtonControl footLower { get; private set; }

    // weight shift controls
    [InputControl(layout = "Axis", displayName = "Weight Shift X-Axis")]
    public AxisControl weightShiftX { get; private set; }

    [InputControl(layout = "Button", displayName = "Weight Shift Left")]
    public ButtonControl weightShiftLeft { get; private set; }

    [InputControl(layout = "Button", displayName = "Weight Shift Right")]
    public ButtonControl weightShiftRight { get; private set; }

    // 4 steo
    [InputControl(layout = "Button", displayName = "Quadrant One")]
    public ButtonControl quadrantOne { get; private set; }

    [InputControl(layout = "Button", displayName = "Quadrant Two")]
    public ButtonControl quadrantTwo { get; private set; }

    [InputControl(layout = "Button", displayName = "Quadrant Three")]
    public ButtonControl quadrantThree { get; private set; }

    [InputControl(layout = "Button", displayName = "Quadrant Four")]
    public ButtonControl quadrantFour { get; private set; }

    [InputControl(layout = "Button", displayName = "All Quadrants Complete")]
    public ButtonControl allQuadrantsComplete { get; private set; }

    // hip abduction
    [InputControl(layout = "Button", displayName = "Left Hip Abducted")]
    public ButtonControl leftHipAbducted { get; private set; }

    [InputControl(layout = "Button", displayName = "Right Hip Abducted")]
    public ButtonControl rightHipAbducted { get; private set; }

    [InputControl(layout = "Axis", displayName = "Abduction Distance")]
    public AxisControl abductionDistance { get; private set; }

    [InputControl(layout = "Axis", displayName = "Left Foot Height")]
    public AxisControl leftFootHeight { get; private set; }

    [InputControl(layout = "Axis", displayName = "Right Foot Height")]
    public AxisControl rightFootHeight { get; private set; }

    // squat controls
    [InputControl(layout = "Axis", displayName = "Squat Tracking Y-Axis")]
    public AxisControl squatTrackingY { get; private set; }
    [InputControl(layout = "Button", displayName = "Squat Started")]
    public ButtonControl squatStarted { get; private set; }

    [InputControl(layout = "Button", displayName = "Squat Completed")]
    public ButtonControl squatCompleted { get; private set; }

    protected override void FinishSetup()
    {
        // connects input to receive data
        base.FinishSetup(); // Call base.FinishSetup() first

        footHeight = GetChildControl<AxisControl>("footHeight");
        footRaise = GetChildControl<ButtonControl>("footRaise");
        footLower = GetChildControl<ButtonControl>("footLower");

        weightShiftX = GetChildControl<AxisControl>("weightShiftX");
        weightShiftLeft = GetChildControl<ButtonControl>("weightShiftLeft");
        weightShiftRight = GetChildControl<ButtonControl>("weightShiftRight");

        quadrantOne = GetChildControl<ButtonControl>("quadrantOne");
        quadrantTwo = GetChildControl<ButtonControl>("quadrantTwo");
        quadrantThree = GetChildControl<ButtonControl>("quadrantThree");
        quadrantFour = GetChildControl<ButtonControl>("quadrantFour");
        allQuadrantsComplete = GetChildControl<ButtonControl>("allQuadrantsComplete");

        leftHipAbducted = GetChildControl<ButtonControl>("leftHipAbducted");
        rightHipAbducted = GetChildControl<ButtonControl>("rightHipAbducted");
        abductionDistance = GetChildControl<AxisControl>("abductionDistance");
        leftFootHeight = GetChildControl<AxisControl>("leftFootHeight");
        rightFootHeight = GetChildControl<AxisControl>("rightFootHeight");

        squatTrackingY = GetChildControl<AxisControl>("squatTrackingY");
        squatStarted = GetChildControl<ButtonControl>("squatStarted");
        squatCompleted = GetChildControl<ButtonControl>("squatCompleted");

        Debug.Log("CapturyInput setup complete");
    }

    public static void Register()
    {
        // registers captury input as device
        InputSystem.RegisterLayout<CapturyInput>(
            matches: new InputDeviceMatcher().WithInterface("Custom"),
            name: "CapturyInput");

        Debug.Log("CapturyInput layout registered with Input System");
    }
}