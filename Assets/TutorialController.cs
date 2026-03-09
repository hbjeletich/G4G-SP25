using UnityEngine;
using UnityEngine.Video;

public class VideoMenuController : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Menu UI Elements to Hide")]
    public GameObject[] menuElements;

    [Header("Optional: Video Display Object")]
    public GameObject videoDisplayObject;

    private bool videoWasPlaying = false;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
    }

    // Detects the moment the video starts playing each frame
    void Update()
    {
        if (videoPlayer.isPlaying && !videoWasPlaying)
        {
            videoWasPlaying = true;
            HideMenu();
        }
    }

    private void HideMenu()
    {
        foreach (GameObject element in menuElements)
        {
            if (element != null)
                element.SetActive(false);
        }

        if (videoDisplayObject != null)
            videoDisplayObject.SetActive(true);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoWasPlaying = false;

        if (videoDisplayObject != null)
            videoDisplayObject.SetActive(false);

        foreach (GameObject element in menuElements)
        {
            if (element != null)
                element.SetActive(true);
        }
    }
}