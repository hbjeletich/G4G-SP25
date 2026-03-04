using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example save log button script.
/// </summary>
[RequireComponent(typeof(Button))]
public class SaveLogButton : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        // does not work in build
#if UNITY_EDITOR
    gameObject.SetActive(true);
#else
    gameObject.SetActive(false);
#endif
    }
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SaveLog);
    }

    private void SaveLog()
    {
        if(DataLogger.Instance == null)
        {
            Debug.LogWarning("DataLogger instance is null. Cannot stop session.");
            return;
        }

        if(DataLogger.Instance.IsSessionActive == false)
        {
            Debug.LogWarning("DataLogger session is not active. Cannot stop session.");
            return;
        }

        DataLogger.Instance.SaveToFile();
    }
}
