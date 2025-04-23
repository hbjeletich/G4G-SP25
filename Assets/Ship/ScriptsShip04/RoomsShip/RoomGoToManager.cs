using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ship
{
public class RoomGoToManager : MonoBehaviour
{
    public static RoomGoToManager instance; 
    private ScreenFade blackScreenFade;
    private ShipPlayerController playerOverworld;

    private void Awake()
    {
        // Manager needs to persist btwn scenes
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
    
    public void GoToRoom(RoomScriptable currentRoom, string exitingTo)
    {
        Debug.Log("Entering loop");
        RoomExitOptions exit = Exit(currentRoom, exitingTo);

        if (exit != null && exit.targetRoom != null) 
        {
            Debug.Log("Found exit to " + exit.targetRoom.roomName);
            StartCoroutine(HandleRoomTransition(exit.targetRoom, exit.spawnPointName));
        }
        else 
        {
            Debug.LogError("Exit not found or target room is null.");
        }
    }

    private RoomExitOptions Exit(RoomScriptable room, string exitingTo)
    {
        foreach (var exit in room.exits)
        {
            if (exit.exitingTo == exitingTo)
            {
                return exit;
            }
        }
        return null;
    }

    private IEnumerator HandleRoomTransition(RoomScriptable targetRoom, string spawnPointName)
    {
        Debug.Log("Starting room transition to: " + targetRoom.roomName);
        
        blackScreenFade = FindObjectOfType<ScreenFade>();
        if (blackScreenFade == null)
        {
            Debug.LogWarning("blackScreenFade = " + blackScreenFade);
            yield break;
        }
        
        // Check if the music and ambient sound need to be faded out
        MusicPersistance.instance.PreTransitionCheckMusic(targetRoom.music);
        MusicPersistance.instance.PreTransitionCheckAmbient(targetRoom.ambientSound);

        // Black screen fade in
        blackScreenFade.StartCoroutine(blackScreenFade.BlackFadeIn());
        yield return new WaitForSeconds(blackScreenFade.fadeDuration);

        // Load new room
        StartCoroutine(ChangeRoom(targetRoom, spawnPointName));
    }

    private IEnumerator ChangeRoom(RoomScriptable targetRoom, string spawnPointName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetRoom.roomName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade out black screen
        blackScreenFade = FindObjectOfType<ScreenFade>();
        if (blackScreenFade != null)
        {
            yield return StartCoroutine(HandleNewRoomTransition(targetRoom));
        }

        // Check for player in new room
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            yield break; 
        }

        // Check for registered spawn point
        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
        }

        // Check for music
        if (targetRoom.music != null && MusicPersistance.instance != null)
        {
            MusicPersistance.instance.CheckMusic(targetRoom.music);
        }
        else
        {
            MusicPersistance.instance.StopMusic();
        }

        // Check for ambient sound
        if (targetRoom.ambientSound != null && MusicPersistance.instance != null)
        {
            MusicPersistance.instance.CheckAmbient(targetRoom.ambientSound);
        }
        else
        {
            MusicPersistance.instance.StopAmbient();
        }
    }

    private IEnumerator HandleNewRoomTransition(RoomScriptable targetRoom)
    {
        if (blackScreenFade == null)
        {
            yield break;
        }
        blackScreenFade.fadeCanvasGroup.alpha = 1;

        playerOverworld = FindObjectOfType<ShipPlayerController>();
        if (playerOverworld != null)
        {
            playerOverworld.EnablePlayerController();
        }
        
        if (targetRoom.roomName == "ShipVillage")
        {
            yield return blackScreenFade.StartCoroutine(blackScreenFade.BlackFadeOutVillage());
        }
        else
        {
            blackScreenFade.StartCoroutine(blackScreenFade.BlackFadeOut());
        }
    }

    #region Buttons
    public void OnTutorialButtonPressed()
    {
        StartCoroutine(EnterTutorialCoroutine());
    }

    private IEnumerator EnterTutorialCoroutine()
    {
        Debug.Log("Starting tutorial transition");

        blackScreenFade = FindObjectOfType<ScreenFade>();
        if (blackScreenFade == null)
        {
            Debug.Log("blackScreenFade = " + blackScreenFade);
            yield break;
        }

        // Black screen fade in
        blackScreenFade.StartCoroutine(blackScreenFade.BlackFadeIn());
        yield return new WaitForSeconds(blackScreenFade.fadeDuration);

        // Load tutorial scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("ShipTutorial", LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade out black screen
        blackScreenFade = FindObjectOfType<ScreenFade>();
        if (blackScreenFade != null)
        {
            blackScreenFade.StartCoroutine(blackScreenFade.BlackFadeOut());
        }

        // Enable player controller in tutorial
        playerOverworld = FindObjectOfType<ShipPlayerController>();
        if (playerOverworld != null)
        {
            playerOverworld.EnablePlayerController();
        }
    }
    #endregion
}
}
