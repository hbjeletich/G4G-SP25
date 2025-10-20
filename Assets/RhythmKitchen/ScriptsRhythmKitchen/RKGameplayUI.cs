using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKGameplayUI : MonoBehaviour
{
    public void CompleteDish()
    {
        SceneManager.LoadScene("RKCompletedDish");
    }
}
