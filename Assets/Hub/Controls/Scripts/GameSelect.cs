using System.Collections;
using System.Collections.Generic;
using Captury;
using Swimming;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSelect : MonoBehaviour
{
    public static GameSelect _instance;
    public string host = "127.0.0.1";

    // find obj of type CapturyNetworkPlugin

    public static GameSelect Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameSelect>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameSelect");
                    _instance = singletonObject.AddComponent<GameSelect>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        CapturySetup();
    }
    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void CapturySetup()
    {
        CapturyNetworkPlugin capturyNetworkPlugin = FindObjectOfType<CapturyNetworkPlugin>();

        if (capturyNetworkPlugin != null)
        {
            capturyNetworkPlugin.host = host;
        }
    }
}
