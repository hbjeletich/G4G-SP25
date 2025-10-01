using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableRoom", menuName = "Room")]
public class RoomScriptable : ScriptableObject
{
    public string roomName; 
    public AudioClip music;
    public AudioClip ambientSound;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float ambientVolume = 1f;
    public string spawnPointName;
    public RoomExitOptions[] exits;
}

[System.Serializable]
public class RoomExitOptions
{
    public string exitingTo; 
    public string spawnPointName;
    public RoomScriptable targetRoom; 
}
