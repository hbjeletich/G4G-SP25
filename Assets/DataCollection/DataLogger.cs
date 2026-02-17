using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Singleton DataLogger for Gaitway Arcade.
/// Any script can call DataLogger.Instance.LogData() to record events.
/// Data is saved to CSV on session end or application quit.
/// </summary>
public class DataLogger : MonoBehaviour
{
    public static DataLogger Instance { get; private set; }

    [Header("Session Info")]
    [SerializeField] private string participantID = "";
    [SerializeField] private int sessionNumber = 1;
    private bool sessionActive = false;

    [Header("Settings")]
    [SerializeField] private string outputFolder = "GameData";
    [SerializeField] private bool logToConsole = true;

    // Internal data storage
    private List<LogEntry> logEntries = new List<LogEntry>();
    private DateTime sessionStartTime;
    private string currentScene = "";

    public event Action OnSessionStarted;
    public bool IsSessionActive => sessionActive;

    // Struct for each log entry
    [Serializable]
    private struct LogEntry
    {
        public string timestamp;
        public float sessionTime;
        public string scene;
        public string eventType;
        public string eventData;

        public LogEntry(string timestamp, float sessionTime, string scene, string eventType, string eventData)
        {
            this.timestamp = timestamp;
            this.sessionTime = sessionTime;
            this.scene = scene;
            this.eventType = eventType;
            this.eventData = eventData;
        }

        public string ToCSV()
        {
            // Escape any commas in eventData
            string safeData = eventData.Contains(",") ? $"\"{eventData}\"" : eventData;
            return $"{timestamp},{sessionTime:F3},{scene},{eventType},{safeData}";
        }
    }

    // Scene time tracking
    private float sceneEnterTime = 0f;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        sceneEnterTime = Time.time;

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        LogData("Session", "SessionEnd");
        SaveToFile();
    }

    /// <summary>
    /// Call this from your menu to initialize the session with participant info.
    /// Session number is automatically determined based on existing files.
    /// </summary>
    public void StartSession(string participantID)
    {
        this.participantID = participantID;
        this.sessionNumber = GetNextSessionNumber(participantID);
        this.sessionStartTime = DateTime.Now;
        this.logEntries.Clear();

        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        LogData("Session", "SessionStart", $"ParticipantID:{participantID},SessionNumber:{sessionNumber}");

        if (logToConsole)
            Debug.Log($"[DataLogger] Session started for {participantID}, session #{sessionNumber}");

        OnSessionStarted?.Invoke();
        sessionActive = true;
    }

    /// <summary>
    /// Scans existing data files to determine the next session number for a participant.
    /// </summary>
    private int GetNextSessionNumber(string participantID)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, outputFolder);
        
        if (!Directory.Exists(folderPath))
        {
            return 1;
        }

        // Look for files matching pattern: GA_[ParticipantID]_[SessionNumber]_*.csv
        string searchPattern = $"GA_{participantID}_*.csv";
        string[] existingFiles = Directory.GetFiles(folderPath, searchPattern);

        if (existingFiles.Length == 0)
        {
            return 1;
        }

        int highestSession = 0;
        foreach (string filePath in existingFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            // Parse: GA_[ID]_[SessionNumber]_[DateTime]
            string[] parts = fileName.Split('_');
            
            // parts[0] = "GA", parts[1] = participantID, parts[2] = sessionNumber
            if (parts.Length >= 3 && int.TryParse(parts[2], out int sessionNum))
            {
                if (sessionNum > highestSession)
                {
                    highestSession = sessionNum;
                }
            }
        }

        return highestSession + 1;
    }

    /// <summary>
    /// Main logging method. Call from any script.
    /// </summary>
    /// <param name="eventType">Category of event (e.g., "Input", "Game", "Minigame", "Session")</param>
    /// <param name="eventName">Specific event name (e.g., "Jump", "LevelStart", "ScoreUpdate")</param>
    /// <param name="eventData">Optional additional data as string</param>
    public void LogData(string eventType, string eventName, string eventData = "")
    {
        if (string.IsNullOrEmpty(participantID) || !sessionActive)
        {
            if (logToConsole)
                Debug.LogWarning("[DataLogger] Attempting to log without starting session. Call StartSession() first.");
            return;
        }

        float sessionTime = (float)(DateTime.Now - sessionStartTime).TotalSeconds;
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        var entry = new LogEntry(timestamp, sessionTime, currentScene, $"{eventType}:{eventName}", eventData);
        logEntries.Add(entry);

        if (logToConsole)
            Debug.Log($"[DataLogger] {entry.eventType} | {entry.eventData}");
    }

    /// <summary>
    /// Convenience method for logging input events.
    /// </summary>
    public void LogInput(string inputName, string inputValue = "")
    {
        LogData("Input", inputName, inputValue);
    }

    /// <summary>
    /// Convenience method for logging game events.
    /// </summary>
    public void LogGameEvent(string eventName, string eventData = "")
    {
        LogData("Game", eventName, eventData);
    }

    /// <summary>
    /// Convenience method for logging minigame-specific events.
    /// </summary>
    public void LogMinigameEvent(string minigameName, string eventName, string eventData = "")
    {
        LogData($"Minigame:{minigameName}", eventName, eventData);
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        string previousScene = currentScene;
        currentScene = scene.name;

        if (!string.IsNullOrEmpty(participantID) && sessionActive)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            LogData("Scene", "SceneLoaded", $"From:{previousScene},To:{currentScene},Time:{timestamp}");
        }
    }

    /// <summary>
    /// Saves all logged data to a CSV file.
    /// Called automatically on application quit, but can be called manually.
    /// </summary>
    public void SaveToFile()
    {
        if (logEntries.Count == 0)
        {
            if (logToConsole)
                Debug.Log("[DataLogger] No entries to save.");
            return;
        }

        // Create output directory
        string folderPath = Path.Combine(Application.persistentDataPath, outputFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Generate filename: GA_[ParticipantID]_[SessionNumber]_[DateTime].csv
        string dateStr = sessionStartTime.ToString("yyyyMMdd_HHmmss");
        string filename = $"GA_{participantID}_{sessionNumber}_{dateStr}.csv";
        string filePath = Path.Combine(folderPath, filename);

        // Build CSV content
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("ParticipantID,SessionNumber,Timestamp,SessionTime,Scene,EventType,EventData");

        foreach (var entry in logEntries)
        {
            sb.AppendLine($"{participantID},{sessionNumber},{entry.ToCSV()}");
        }

        // Write to file
        try
        {
            File.WriteAllText(filePath, sb.ToString());
            if (logToConsole)
                Debug.Log($"[DataLogger] Data saved to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataLogger] Failed to save data: {e.Message}");
        }
    }

    /// <summary>
    /// Returns the path where data files are saved.
    /// </summary>
    public string GetOutputPath()
    {
        return Path.Combine(Application.persistentDataPath, outputFolder);
    }

    /// <summary>
    /// Returns current session info for display.
    /// </summary>
    public (string participantID, int sessionNumber, int entryCount) GetSessionInfo()
    {
        return (participantID, sessionNumber, logEntries.Count);
    }
}