using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DataLoggerInput : MonoBehaviour
{
    private string participantID = null;
    [Header("UI Elements")]
    public TMP_InputField participantIDInputField;
    public Button startLoggingButton;
    public Button skipLoggingButton;
    public IPSelect ipSelect;
    [Header("Helper Text")]
    public TextMeshProUGUI helperText;
    public Color colorNormal = Color.white;
    public Color colorWarning = Color.red;
    public float warningTime = 2f;

    private void Start()
    {
        if (participantIDInputField != null)
        {
            participantIDInputField.onEndEdit.AddListener(OnParticipantIDEntered);
        }

        if (startLoggingButton != null)
        {
            startLoggingButton.onClick.AddListener(StartLogging);
        }
    }

    private void OnParticipantIDEntered(string input)
    {
        participantID = input;
    }

    public void StartLogging()
    {
        if (participantID != null)
        {
            DataLogger.Instance.StartSession(participantID);
            HideInputs();
        }
        else
        {
            helperText.text = "Please enter a Participant ID.";
            helperText.color = colorWarning;
            StartCoroutine(BackToWhite());
            Debug.LogWarning("Participant ID is null. Cannot start logging session.");
        }
    }

    public void SkipLogging()
    {
        HideInputs();
    }

    public void HideInputs()
    {
        if (participantIDInputField != null)
        {
            participantIDInputField.gameObject.SetActive(false);
        }

        if (startLoggingButton != null)
        {
            startLoggingButton.gameObject.SetActive(false);
        }

        if (skipLoggingButton != null)
        {
            skipLoggingButton.gameObject.SetActive(false);
        }

        if(helperText != null)
        {
            helperText.gameObject.SetActive(false);
        }
    }

    private IEnumerator BackToWhite()
    {
        float elapsedTime = 0f;
        while (elapsedTime < warningTime)
        {
            helperText.color = Color.Lerp(colorWarning, colorNormal, elapsedTime / warningTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        helperText.color = colorNormal;
    }
}
