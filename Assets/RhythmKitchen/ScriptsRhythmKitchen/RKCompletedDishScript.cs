using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKCompletedDishScript : MonoBehaviour
{
    public void BackMainMenu()
    {
        SceneManager.LoadScene("RKMainMenu");
    }

    public void GoToSongSelect()
    {
        SceneManager.LoadScene("RKSongSelectMenu");
    }

}

