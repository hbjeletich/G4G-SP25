using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SimpleMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameplaySceneName = "GameScene"; // Name of your game scene

    [Header("UI Buttons")]
    public Button playButton;
    public Button quitButton;

    [Header("Hover Animation Settings")]
    public float hoverScale = 1.15f;  // Scale of hovered button
    public float shrinkScale = 0.9f;  // Scale of unhovered buttons
    public float smoothSpeed = 6f;    // Lerp speed

    private readonly List<Button> allButtons = new List<Button>();
    private readonly Dictionary<Button, Vector3> targetScales = new Dictionary<Button, Vector3>();

    private void Start()
    {
        // ✅ Verify that buttons are assigned
        if (playButton == null || quitButton == null)
        {
            Debug.LogError("❌ SimpleMenu: Buttons not assigned in the Inspector.");
            return;
        }

        // Hook up buttons
        playButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);

        allButtons.Add(playButton);
        allButtons.Add(quitButton);

        // Initialize each button
        foreach (var btn in allButtons)
        {
            if (btn == null) continue;

            // Store base scale
            targetScales[btn] = Vector3.one;

            // Add triggers for hover enter and exit
            var trigger = btn.gameObject.AddComponent<EventTrigger>();

            var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((data) => { OnHover(btn); });
            trigger.triggers.Add(entryEnter);

            var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener((data) => { OnExit(btn); });
            trigger.triggers.Add(entryExit);
        }
    }

    private void Update()
    {
        // Animate the scaling for all buttons *and their text*
        foreach (var btn in allButtons)
        {
            if (btn == null) continue;

            Vector3 current = btn.transform.localScale;
            Vector3 target = targetScales[btn];
            btn.transform.localScale = Vector3.Lerp(current, target, Time.deltaTime * smoothSpeed);

            // Scale text slightly too, for a floaty “feel”
            Text text = btn.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.transform.localScale = Vector3.Lerp(text.transform.localScale, target, Time.deltaTime * (smoothSpeed * 1.2f));
            }
        }
    }

    // === Scene and Quit actions ===
    private void StartGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // === Hover Effects ===
    private void OnHover(Button btn)
    {
        foreach (var b in allButtons)
        {
            targetScales[b] = (b == btn) ? Vector3.one * hoverScale : Vector3.one * shrinkScale;
        }
    }

    private void OnExit(Button btn)
    {
        foreach (var b in allButtons)
        {
            targetScales[b] = Vector3.one;
        }
    }
}
