using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uintyEngine.SceneManagement;

public class Settings_Script : MonoBehaviour
{
   public void playgame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
