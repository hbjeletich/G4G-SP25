using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRoomCollection", menuName = "RoomCollection")]
public class RoomCollectionScriptable : ScriptableObject
{
    public List<RoomScriptable> rooms;
}
