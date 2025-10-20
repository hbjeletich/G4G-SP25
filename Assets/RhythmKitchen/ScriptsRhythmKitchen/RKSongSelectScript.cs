using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKSongSelectScript : MonoBehaviour
{
    public void BackMainMenu()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress");
        SceneManager.LoadScene("RKMainMenu");
    }

    public void SelectSong(string songName)
    {
        RKAudioManager.Instance.PlaySFX("RecordScratch");
        RKAudioManager.Instance.StopMusic();
        SceneManager.LoadScene(songName);
    }
}
