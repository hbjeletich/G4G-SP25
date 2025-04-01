using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GolfReloadManager : MonoBehaviour
{
    public static GolfReloadManager instance { get; private set; }

    private void Awake(){
        if (instance != null && instance != this){
            //only allow one instance of this singleton. Destroy duplicates
            Destroy(this);
        } else {
            instance = this;
        }
    }

    public void reloadSceneInBackground(){
         StartCoroutine(ReloadSceneCoroutine());
    }

    IEnumerator ReloadSceneCoroutine(){
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        while (!asyncLoad.isDone){
            yield return null;
        }
    }
}
