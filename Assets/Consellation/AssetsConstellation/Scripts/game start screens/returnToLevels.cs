using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static toLevelsFlag;
using UnityEngine.SceneManagement;

public class returnToLevels : MonoBehaviour
{
    public string startMenuSceneName = "startScreen";

    public void ExitLevel()
    {
        MenuReturnState.ReturnToLevelSelect = true;
        SceneManager.LoadScene(startMenuSceneName);
    }
}