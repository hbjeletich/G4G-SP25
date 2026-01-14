using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IPSelect : MonoBehaviour
{
    public Button limButton;
    public Button homeButton;

    void Start()
    {
        limButton.onClick.AddListener(() => LIMIP());
        homeButton.onClick.AddListener(() => HomeIP());
    }

    void LIMIP()
    {
        GameSelect.Instance.host = "192.168.10.106";
        GameSelect.Instance.CapturySetup();
        GoToNextScene();
    }

    void HomeIP()
    {
        GameSelect.Instance.host = "127.0.0.1";
        GameSelect.Instance.CapturySetup();
        GoToNextScene();
    }

    void GoToNextScene()
    {
        StartCoroutine(WaitAndGo());
    }

    private IEnumerator WaitAndGo()
    {
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("NewGameSelect");
    }
}
