using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LengthRange
{
    public float min;
    public float max;
}

[CreateAssetMenu(fileName = "New Fish", menuName = "Fish")]
public class FishData : ScriptableObject
{
    public string fishName;
    public Sprite fishSprite;
    public string rarity;
    public LengthRange lengthRange;
    public string funFact;
}