using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

[InputControlLayout(displayName = "Captury Input")]
public class CapturyInput : InputDevice
{
    [InputControl(layout = "Axis")]
    public AxisControl footHeight { get; private set; }

    [InputControl(layout = "Button")]
    public ButtonControl footRaise { get; private set; }

    [InputControl(layout = "Button")]
    public ButtonControl footLower { get; private set; }

    protected override void FinishSetup()
    {
        footHeight = GetChildControl<AxisControl>("footHeight");
        footRaise = GetChildControl<ButtonControl>("footRaise");
        footLower = GetChildControl<ButtonControl>("footLower");

        base.FinishSetup();
    }

    public static void Register()
    {
        InputSystem.RegisterLayout<CapturyInput>(
            matches: new InputDeviceMatcher().WithInterface("CapturyInput"));
    }
}
