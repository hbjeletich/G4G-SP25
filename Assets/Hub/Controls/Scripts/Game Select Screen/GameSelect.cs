using System.Collections;
using System.Collections.Generic;
using Captury;
using Swimming;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class GameSelect : MonoBehaviour
{
    // host to overwrite IP of captury network plugin
    [Header("Captury Settings")]
    public string host = "127.0.0.1";

    // sound
    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioSource localSource;

    public static GameSelect _instance; // singleton
    public AudioSource AudioSource { get; private set; }

    public static GameSelect Instance
    {
        // singleton
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
        // singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        localSource = GetComponent<AudioSource>();
        if(localSource == null)
        {
            localSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        CapturySetup(); // set host IP on start
    }


    private void Update()
    {
        // exit game logic
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(ExitGameCoroutine());
        }
    }

    private IEnumerator ExitGameCoroutine()
    {
        // go back to game select
        yield return SceneManager.LoadSceneAsync("GameSelectScene", LoadSceneMode.Single);
        yield return null;
        
        // stop audio
        if (AudioSource != null && AudioSource.isPlaying)
        {
            AudioSource.Stop();
        }

        if(localSource != null && localSource.isPlaying)
        {
            localSource.Stop();
        }

        UnityEngine.Rendering.VolumeManager.instance.ResetMainStack();
    }


    public void OpenScene(string sceneName)
    {
        // go to new game scene
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // async load scene before load
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // delay one more frame to ensure scene camera/volume is fully initialized
        yield return null;

        UnityEngine.Rendering.VolumeManager.instance.ResetMainStack();
    }


    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;  // Stop in editor
        #else
            Application.Quit();  // Stop in build
        #endif
    }

    public void HoverButton(float pitch = 1.0f)
    {
        // play sound on button hover
        if (audioSource != null && hoverSound != null)
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(hoverSound);
        }
        else 
        {
            Debug.Log("Missing AudioSource or AudioClip on GameSelect script.");
        }
    }

    public void CapturySetup()
    {
        // set captury host
        CapturyNetworkPlugin capturyNetworkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (capturyNetworkPlugin != null)
        {
            capturyNetworkPlugin.host = host;
        }
    }
}