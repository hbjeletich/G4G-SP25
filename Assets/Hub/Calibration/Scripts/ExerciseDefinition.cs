using UnityEngine;

[System.Serializable]
public class ExerciseDefinition
{
    public string exerciseName = "Exercise";
    public ExerciseType exerciseType;
    public int animIndex = 1;

    [TextArea(2, 4)]
    public string instructionText = "Watch the demo, then try it yourself!";
}

public enum ExerciseType
{
    HipAbduction,
    WeightShift,
    LegLift,
    Squat
}