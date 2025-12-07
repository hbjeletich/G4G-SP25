using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGame", menuName = "Game Select/Game Data")]
public class GameData : ScriptableObject
{
    public string gameTitle;
    public string targetSceneName;
    public Sprite gameImage;
}