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
        if (!gameOver)
        {
            gameOver = true;
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f); 
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("ShipTitleScreen"); 
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

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







