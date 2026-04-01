using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationGuard : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Scene to load after all exercises are complete.")]
    public string nextSceneName = "NewGameSelect";

    [Tooltip("Extra buffer (seconds) after calibration delay to wait before restoring.")]
    [SerializeField] private float restoreBuffer = 0.2f;

    // singleton
    private static CalibrationGuard instance;
    public static CalibrationGuard Instance => instance;

    // saved calibration data: keyed by module type name (string) for safety across instances
    private Dictionary<string, CalibrationSnapshot> savedSnapshots = new Dictionary<string, CalibrationSnapshot>();

    public bool HasGoodCalibration => savedSnapshots.Count > 0;

    private Coroutine restoreCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SaveCalibration()
    {
        var manager = MotionTrackingManager.Instance;
        if (manager == null)
        {
            Debug.LogError("CalibrationGuard: No MotionTrackingManager found!");
            return;
        }

        if (!manager.IsSystemCalibrated)
        {
            Debug.LogWarning("CalibrationGuard: System is not calibrated yet — nothing to save.");
            return;
        }

        savedSnapshots.Clear();

        // Save from each known module type
        SaveModuleCalibration<TorsoTrackingModule>(manager);
        SaveModuleCalibration<FootTrackingModule>(manager);
        SaveModuleCalibration<ArmTrackingModule>(manager);
        SaveModuleCalibration<HeadTrackingModule>(manager);
        SaveModuleCalibration<BalanceTrackingModule>(manager);

        Debug.Log($"CalibrationGuard: Saved {savedSnapshots.Count} module calibrations.");
    }

    public void ClearCalibration()
    {
        savedSnapshots.Clear();
        Debug.Log("CalibrationGuard: Cleared saved calibrations.");
    }

    private void SaveModuleCalibration<T>(MotionTrackingManager manager) where T : MotionTrackingModule
    {
        var module = manager.GetModule<T>();
        if (module != null && module.IsCalibrated && module.CurrentCalibration != null)
        {
            savedSnapshots[typeof(T).Name] = module.CurrentCalibration.Clone();
            Debug.Log($"CalibrationGuard: Saved calibration for {typeof(T).Name}");
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (!HasGoodCalibration) return;

        // Don't restore in the hub scene — that's where we create the calibration
        // The hub flow controller will handle calibration there
        if (restoreCoroutine != null)
        {
            StopCoroutine(restoreCoroutine);
        }

        restoreCoroutine = StartCoroutine(WaitAndRestoreCalibration());
    }

    private IEnumerator WaitAndRestoreCalibration()
    {
        // Wait for the MotionTrackingManager to exist
        float timeout = 10f;
        float elapsed = 0f;

        while (MotionTrackingManager.Instance == null && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        var manager = MotionTrackingManager.Instance;
        if (manager == null)
        {
            Debug.LogWarning("CalibrationGuard: No MotionTrackingManager found in scene — skipping restore.");
            yield break;
        }

        // Read the calibration delay from the manager's config
        float calibrationDelay = 0f;
        if (manager.Config != null)
        {
            calibrationDelay = manager.Config.calibrationDelay;
            Debug.Log($"CalibrationGuard: Config calibration delay is {calibrationDelay}s");
        }

        // Wait for calibration to complete
        // First wait for it to start calibrating
        float waitForCalStart = 0f;
        while (!manager.IsCalibrating && !manager.IsSystemCalibrated && waitForCalStart < timeout)
        {
            waitForCalStart += Time.deltaTime;
            yield return null;
        }

        // Now wait for the calibration delay + buffer
        if (manager.IsCalibrating)
        {
            Debug.Log($"CalibrationGuard: Waiting {calibrationDelay + restoreBuffer}s for calibration to finish...");
            yield return new WaitForSeconds(calibrationDelay + restoreBuffer);
        }

        // Final check: wait until calibrated
        elapsed = 0f;
        while (!manager.IsSystemCalibrated && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!manager.IsSystemCalibrated)
        {
            Debug.LogWarning("CalibrationGuard: Timed out waiting for calibration — skipping restore.");
            yield break;
        }

        // Restore our saved calibrations
        RestoreCalibration(manager);
        restoreCoroutine = null;
    }

    private void RestoreCalibration(MotionTrackingManager manager)
    {
        int restored = 0;

        RestoreModuleCalibration<TorsoTrackingModule>(manager, ref restored);
        RestoreModuleCalibration<FootTrackingModule>(manager, ref restored);
        RestoreModuleCalibration<ArmTrackingModule>(manager, ref restored);
        RestoreModuleCalibration<HeadTrackingModule>(manager, ref restored);
        RestoreModuleCalibration<BalanceTrackingModule>(manager, ref restored);

        Debug.Log($"CalibrationGuard: Restored {restored}/{savedSnapshots.Count} module calibrations.");
    }

    private void RestoreModuleCalibration<T>(MotionTrackingManager manager, ref int count) where T : MotionTrackingModule
    {
        string key = typeof(T).Name;
        if (!savedSnapshots.ContainsKey(key)) return;

        var module = manager.GetModule<T>();
        if (module != null)
        {
            module.SetCalibration(savedSnapshots[key].Clone());
            count++;
            Debug.Log($"CalibrationGuard: Restored calibration for {key}");
        }
        else
        {
            Debug.LogWarning($"CalibrationGuard: Module {key} not found in this scene's manager — skipping.");
        }
    }
}