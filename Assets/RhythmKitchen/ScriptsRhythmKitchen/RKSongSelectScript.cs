using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKSongSelectScript : MonoBehaviour
{
    public void BackMainMenu()
    {
        SceneManager.LoadScene("RKMainMenu");
    }

    public void SelectSong(string songName)
    {
        SceneManager.LoadScene(songName);
    }
}
