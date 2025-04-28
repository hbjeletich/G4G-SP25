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
    [Header("Captury Settings")]
    public string host = "192.168.10.106";

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip hoverSound;

    public static GameSelect _instance;
    public AudioSource AudioSource { get; private set; }

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
        SetupButtons();
    }

    private void SetupButtons()
    {
        /*// Find all game selection buttons in the scene
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            if(button != null)
            {
                SceneButton sceneButton = button.GetComponent<SceneButton>();
                if (sceneButton != null)
                {
                    button.onClick.RemoveAllListeners();
                    string sceneName = sceneButton.targetSceneName;

                    button.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
                }
            }
        }*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(ExitGameCoroutine());
        }
    }

    private IEnumerator ExitGameCoroutine()
    {
        yield return SceneManager.LoadSceneAsync("GameSelectScene", LoadSceneMode.Single);
        yield return null;
        
        if (AudioSource != null && AudioSource.isPlaying)
        {
            AudioSource.Stop();
        }
        UnityEngine.Rendering.VolumeManager.instance.ResetMainStack();
    }


    public void OpenScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Delay one more frame to ensure scene camera/volume is fully initialized
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
        CapturyNetworkPlugin capturyNetworkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (capturyNetworkPlugin != null)
        {
            capturyNetworkPlugin.host = host;
        }
    }
}