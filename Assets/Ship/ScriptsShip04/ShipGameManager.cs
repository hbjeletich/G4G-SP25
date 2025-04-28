using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.SceneManagement;

namespace Ship
{
public class ShipGameManager : MonoBehaviour
{
    [HideInInspector] public static ShipGameManager instance; // Singleton instance
    [HideInInspector] public bool gameOver = false; 
    public static float totalPoints = 0; // Total points across all rooms

    [Header("Title Button Data")]    
    public RoomScriptable currentRoom;
    public RoomScriptable mainLevelScriptable;  
    public RoomScriptable tutorialLevelScriptable;
    public string exitingTo1;
    public string exitingTo2;

    private void Awake()
    {
        if (ShipGameManager.instance == null)
        {
            ShipGameManager.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (ShipGameManager.instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void TriggerGameOver()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "ShipTitleScreen")
        {
            return;
        }
        if (!gameOver && scene.name == "ShipMainLevel")
        {
            if (mainLevelScriptable != null)
            {
                RoomGoToManager.instance.GoToRoom(mainLevelScriptable, "ShipTitleScreen");
            }
            else
            {
                Debug.LogError("Main Level Scriptable is not assigned.");
            }
        }
        if (!gameOver && scene.name == "ShipTutorial")
        {
            if (tutorialLevelScriptable != null)
            {
                RoomGoToManager.instance.GoToRoom(tutorialLevelScriptable, "ShipTitleScreen");
            }
            else
            {
                Debug.LogError("Tutorial Level Scriptable is not assigned.");
            }
        }
    }

    // private IEnumerator GameOver()
    // {
    //     yield return new WaitForSeconds(1f); 
    //     AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("ShipTitleScreen"); 
    //     while (!asyncLoad.isDone)
    //     {
    //         yield return null;
    //     }
    // }

    // Coroutine to handle scene loading
    private IEnumerator LoadRoomScene(RoomScriptable targetRoom)
    {
        // Load target scene asynchronously
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetRoom.roomName);

        // Wait until the scene is loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Find player in the new scene and position them at the spawn point
        var player = FindObjectOfType<ShipPlayerController>();
        if (player != null)
        {
            var spawnPoint = GameObject.Find(targetRoom.spawnPointName);
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log($"Player moved to spawn point: {targetRoom.spawnPointName} in scene {targetRoom.roomName}");
            }
            else
            {
                Debug.LogWarning($"Spawn point {targetRoom.spawnPointName} not found in scene {targetRoom.roomName}.");
            }
        }
    }

    public void OnTutorialButtonPressed()
    {
        RoomGoToManager.instance.GoToRoom(currentRoom, exitingTo1);
    }   

    public void OnStartButtonPressed()
    {
        RoomGoToManager.instance.GoToRoom(currentRoom, exitingTo2);
    }
}

[System.Serializable]
public static class RoomManager
{
    public static Dictionary<string, RoomScriptable> RoomDictionary;

    public static void Initialize(RoomCollectionScriptable collection)
    {
        RoomDictionary = new Dictionary<string, RoomScriptable>();

        foreach (var room in collection.rooms)
        {
            if (!RoomDictionary.ContainsKey(room.roomName))
            {
                RoomDictionary.Add(room.roomName, room);
            }
            else
            {
                Debug.LogError($"Duplicate room name detected: {room.roomName}");
            }
        }
        Debug.Log($"RoomManager initialized with {RoomDictionary.Count} rooms.");
    }
}
}







