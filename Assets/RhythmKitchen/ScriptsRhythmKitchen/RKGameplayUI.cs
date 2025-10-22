using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKGameplayUI : MonoBehaviour
{
    public void CompleteDish()
    {
        var am = RKAudioManager.Instance;
        {
            if (am != null) // is not empty
            {
                am.PlaySFX("ButtonPress");
                am.PlaySFX("Shimmer");
            }
            else
            {
                Debug.LogWarning("[RKGameplayUI] AudioManager missing: loading scene anyway.");
            }
            SceneManager.LoadScene("RKCompletedDish");
        }
    }
}
