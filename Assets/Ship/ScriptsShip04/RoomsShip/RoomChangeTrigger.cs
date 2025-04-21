using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;

namespace Ship
{
public class RoomChangeTrigger : MonoBehaviour
{
    public RoomScriptable currentRoom; 
    public string exitingTo; 
    private ShipPlayerController playerOverworld;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger for room change: " + exitingTo);
            // playerOverworld.UpdatePlayerRoom(exitingTo);
            // playerOverworld.DisablePlayerController();
            RoomGoToManager.instance.GoToRoom(currentRoom, exitingTo);

            // playerOverworld = other.GetComponent<ShipPlayerController>();    
        }
    }
}
}

