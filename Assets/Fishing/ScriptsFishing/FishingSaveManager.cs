using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Fishing
{
public class FishingSaveManager : MonoBehaviour
{
    public static FishingSaveManager instance;
    private static GameData currentGameData;
    private string saveFilePath; 
    private int currentSaveSlot = 1;
    private float tempElapsedTime = 0f; 

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void Update()
    {
        AddElapsedTime(Time.deltaTime);
    }

    public void AddElapsedTime(float time)
    {
        if (currentGameData != null)
        {
            currentGameData.elapsedTime += time;
        }
        else
        {
            tempElapsedTime += time;
        }
    }

    public void SetCurrentSlot(int slot)
    {
        currentSaveSlot = slot;
        Debug.Log($"Active save slot set to: {slot}");
    }

    public static GameData GetGameData(int slot = -1)
    {
        // Default to currentSaveSlot if no slot is specified
        if (slot == -1)
        {
            slot = instance.currentSaveSlot;
        }

        string filePath = instance.GetFilePath(slot);

        if (File.Exists(filePath))
        {
            string dataToLoad = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(dataToLoad);
        }
        else
        {
            return null;  // No save data found
        }
    }

    public static void SetGameData(GameData aData)
    {
        currentGameData = aData;
    }

    // Default save slot is 1 if one is not provided
    public void SaveGameData()
    {
        SaveGameData(currentSaveSlot); 
    }

    // Helper method to set the current save slot
    public void LoadGameData()
    {
        LoadGameData(currentSaveSlot); 
    }
    
    public void SaveGameData(int slot)
    {
        try
        {
            UpdateGameData(); // Update current game data

            string filePath = GetFilePath(slot);
            string dataToSave = JsonUtility.ToJson(currentGameData);
            File.WriteAllText(filePath, dataToSave);

            Debug.Log($"Game saved to Slot {slot}! Elapsed Time: {currentGameData.elapsedTime}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game to Slot {slot}: {e}");
        }
    }

    public void LoadGameData(int slot)
    {
        try
        {
            string filePath = GetFilePath(slot);

            if (File.Exists(filePath))
            {
                string dataToLoad = File.ReadAllText(filePath);
                currentGameData = JsonUtility.FromJson<GameData>(dataToLoad);

                // Reset temporary time
                tempElapsedTime = 0f;

                RestoreGameData(); 
                Debug.Log($"Game loaded from Slot {slot}!");
            }
            else
            {
                currentGameData = null;
                Debug.LogWarning($"No save file found in Slot {slot}.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game from Slot {slot}: {e}");
        }
    }

    public void CreateNewSaveSlot(int slot)
    {
        if (currentGameData == null) // Only create if no active save data
        {
            currentGameData = new GameData
            {
                elapsedTime = tempElapsedTime 
            };

            SetCurrentSlot(slot); // Update the current save slot
            SaveGameData(slot);   // Save the data to disk

            Debug.Log($"New save slot {slot} created with elapsed time: {tempElapsedTime}");
        }
        else
        {
            Debug.LogWarning($"Save slot {slot} already exists or is being used.");
        }
    }


    private void UpdateGameData()
    {
        var player = FindObjectOfType<FishingPlayerController>();
        currentGameData.elapsedTime += Time.deltaTime;
    }

    private void RestoreGameData()
    {
        if (currentGameData == null)
        {
            Debug.LogWarning("No game data to restore.");
            return;
        }
        else
        {
            StartCoroutine(LoadGame());
        }
    }

    private IEnumerator LoadGame()
    {
        if (currentGameData == null)
        {
            Debug.LogWarning("No game data to load.");
            yield break;
        }

        // Load the scene specified in the saved game data
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("FishingGame");

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Restore player state or other game elements if needed
        var player = FindObjectOfType<FishingPlayerController>();
        if (player != null)
        {
            // player.RestoreCollectedFish(currentGameData.collectedFish);
            Debug.Log("Player state restored.");
        }
        else
        {
            Debug.LogWarning("Player not found in the loaded scene.");
        }
    }

    // Coroutine to handle scene loading
    // private IEnumerator LoadRoomScene(RoomScriptable targetRoom)
    // {
    //     // Load target scene asynchronously
    //     AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetRoom.roomName);

    //     // Wait until the scene is loaded
    //     while (!asyncLoad.isDone)
    //     {
    //         yield return null;
    //     }

    //     // Find player in the new scene and position them at the spawn point
    //     var player = FindObjectOfType<FishingPlayerController>();
    //     if (player != null)
    //     {
    //         var spawnPoint = GameObject.Find(targetRoom.spawnPointName);
    //         if (spawnPoint != null)
    //         {
    //             player.transform.position = spawnPoint.transform.position;
    //             Debug.Log($"Player moved to spawn point: {targetRoom.spawnPointName} in scene {targetRoom.roomName}");
    //         }
    //         else
    //         {
    //             Debug.LogWarning($"Spawn point {targetRoom.spawnPointName} not found in scene {targetRoom.roomName}.");
    //         }
    //     }
    // }

    private string GetFilePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"slot{slot}.json");
    }
}

[System.Serializable]
public class GameData
{
    public float elapsedTime = 0f; 
    public string currentRoom = "";
    public List<string> collectedFish = new List<string>();
}

// [System.Serializable]
// public static class RoomManager
// {
//     public static Dictionary<string, RoomScriptable> RoomDictionary;

//     public static void Initialize(RoomCollectionScriptable collection)
//     {
//         RoomDictionary = new Dictionary<string, RoomScriptable>();

//         foreach (var room in collection.rooms)
//         {
//             if (!RoomDictionary.ContainsKey(room.roomName))
//             {
//                 RoomDictionary.Add(room.roomName, room);
//             }
//             else
//             {
//                 Debug.LogError($"Duplicate room name detected: {room.roomName}");
//             }
//         }

//         Debug.Log($"RoomManager initialized with {RoomDictionary.Count} rooms.");
//     }
// }
}



