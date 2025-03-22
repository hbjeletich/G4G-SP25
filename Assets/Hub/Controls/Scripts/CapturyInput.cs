using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

// Specify the state structure format for this device
public struct CapturyInputState : IInputStateTypeInfo
{
    // Format code must be exactly 4 characters
    public FourCC format => new FourCC('C', 'A', 'P', 'T');

    [InputControl(layout = "Axis")]
    public float footHeight;

    [InputControl(layout = "Button")]
    public float footRaise;

    [InputControl(layout = "Button")]
    public float footLower;
}

[InputControlLayout(stateType = typeof(CapturyInputState), displayName = "Captury Input")]
public class CapturyInput : InputDevice
{
    [InputControl(layout = "Axis", displayName = "Foot Height", usage = "Primary")]
    public AxisControl footHeight { get; private set; }

    [InputControl(layout = "Button", displayName = "Foot Raise")]
    public ButtonControl footRaise { get; private set; }

    [InputControl(layout = "Button", displayName = "Foot Lower")]
    public ButtonControl footLower { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup(); // Call base.FinishSetup() first

        footHeight = GetChildControl<AxisControl>("footHeight");
        footRaise = GetChildControl<ButtonControl>("footRaise");
        footLower = GetChildControl<ButtonControl>("footLower");

        Debug.Log($"CapturyInput setup complete - controls: {footHeight != null}, {footRaise != null}, {footLower != null}");
    }

    public static void Register()
    {
        InputSystem.RegisterLayout<CapturyInput>(
            matches: new InputDeviceMatcher().WithInterface("Custom"),
            name: "CapturyInput");

        Debug.Log("CapturyInput layout registered with Input System");
    }
}