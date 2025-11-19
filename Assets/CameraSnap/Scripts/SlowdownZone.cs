using UnityEngine;

namespace CameraSnap
{
    public class SlowdownZone : MonoBehaviour
    {
        [Header("Slowdown Settings")]
        public float slowedSpeed = 1f;
        // Timer logic moved to ZoneStopTimer component
        [Tooltip("Seconds to wait after exit/completion before despawning animals. Set <= 0 to disable delayed despawn and clear immediately.")]
        public float animalDespawnDelay = 10f;

        [Header("Components")]
        public AnimalSpawner animalSpawner;
        private ZoneStopTimer zoneTimer;
    private UIManager ui => UIManager.Instance;
    // Delay clearing animals by starting a coroutine. We don't cancel scheduled clears because
    // the player will not realistically re-enter a zone without restarting the scene.
    private void ScheduleAnimalClear(float delay)
    {
        if (animalSpawner == null)
        {
            Debug.LogWarning("[SlowdownZone] No AnimalSpawner assigned; cannot schedule clear.");
            return;
        }
        if (delay <= 0f)
        {
            animalSpawner.ClearPreviousAnimals();
            return;
        }
        StartCoroutine(DelayedClearCoroutine(delay));
    }

    private System.Collections.IEnumerator DelayedClearCoroutine(float delay)
    {
        Debug.Log($"[SlowdownZone] Scheduled animal clear in {delay} seconds.");
        yield return new WaitForSeconds(delay);
        if (animalSpawner != null)
        {
            animalSpawner.ClearPreviousAnimals();
            Debug.Log("[SlowdownZone] Cleared animals after delay.");
        }
    }

        [Header("Assigned Animals (optional)")]
        [Tooltip("If populated, these AnimalData entries will be used for this zone (in order). If empty, animals are chosen randomly from GameManager.")]
        public System.Collections.Generic.List<AnimalData> assignedAnimals;

        
//Cart is reduced to slowed speed. The player is allowed to stop the cart. UI icon appears to indicate you need to press key
//or do movement to stop cart. Animals for the zone is spawned in..
        private void Awake()
        {
            // Automatically add ZoneStopTimer if missing
            zoneTimer = GetComponent<ZoneStopTimer>();
            if (zoneTimer == null)
            {
                zoneTimer = gameObject.AddComponent<ZoneStopTimer>();
                Debug.Log($"[SlowdownZone] Added ZoneStopTimer to {gameObject.name}");
            }
            zoneTimer.OnTimeout += HandleTimerTimeout;
        }

        private void OnDestroy()
        {
            if (zoneTimer != null)
                zoneTimer.OnTimeout -= HandleTimerTimeout;
        }

        private void OnTriggerEnter(Collider other)
        {
            CartController cart = other.GetComponentInParent<CartController>();
            if (cart != null)
            {
                // (no cancel needed; re-entering a zone without restarting is unlikely)
                cart.currentZone = this;
                cart.SetSpeed(slowedSpeed);
                cart.AllowStop(true);

                // Show zone UI and prompt the player
                if (ui == null)
                    Debug.LogError("[SlowdownZone] UIManager not found; cannot show zone icon or guide.");
                else
                {
                    ui.SetZoneIconVisible(true);
                    ui.SetGuideState(UIManager.GuideState.Squat);
                }

                if (animalSpawner != null)
                    animalSpawner.SpawnAnimals(assignedAnimals);

                // start the stop timer tracking if timer component exists
                zoneTimer?.StartTracking(cart);
            }
        }
//Resets speed to normal, the player can no longer stop. The UI icon disappears, and the remaining animals in the zone are removed.
private void OnTriggerExit(Collider other)
{
    CartController cart = other.GetComponentInParent<CartController>();
    if (cart != null)
    {
        if (cart.currentZone == this) cart.currentZone = null; // clear it
        cart.ResetSpeed();
        cart.AllowStop(false);

        if (ui == null)
        {
            Debug.LogError("[SlowdownZone] UIManager not found; cannot hide zone icon or guide.");
        }
        else
        {
            ui.SetZoneIconVisible(false);
            ui.HideGuide();
            ui.HideStopCountdown();
        }

    if (animalSpawner != null) ScheduleAnimalClear(animalDespawnDelay);

        // stop timer tracking
        zoneTimer?.StopTracking();
    }
}
//Get list of animals spawned in zone, count how many are captured, if every animal is captured, restore cart ot normal.
        public void CheckForZoneCompletion()
{
    if (animalSpawner == null || GameManager.Instance == null)
        return;

    var animals = animalSpawner.GetSpawnedAnimals();
    if (animals == null || animals.Count == 0)
        return;
//^ prevents method from running if the spawner does not exist, no gamemanager exists, no animals have been spawned..
    int capturedCount = 0;

    foreach (var a in animals)
    {
        if (a == null) continue;
        var behavior = a.GetComponent<AnimalBehavior>();
if (behavior != null && GameManager.Instance.HasCaptured(behavior.animalData.animalName))
    capturedCount++;
//^Iterates over all spawned animals in zone. Skips any destroyed or null objects, checks if animal has animal behavior script
//which is needed for animal data. Uses gamemanager.hascaptured to know if player photographed it. Increases captured count
//for each captured animal.
    }

    if (capturedCount >= animals.Count)
    {
        //Zone is only complete when all spawned animals in the zone have been captured. >= used in case of duplicates or errors.
        //Ensures full completion before proceeding.
        Debug.Log("[SlowdownZone] All zone animals (2) captured! Resuming cart...");

//V restore cart behavior to normal speed, disable stop, find cart object
        CartController cart = FindObjectOfType<CartController>();
        if (cart != null)
        {
            cart.ResetSpeed();    // back to normal
            cart.AllowStop(false);
        }
//hide zone UI
        if (ui == null)
        {
            Debug.LogError("[SlowdownZone] UIManager not found; cannot hide zone icon or guide.");
        }
        else
        {
            ui.SetZoneIconVisible(false);
            ui.HideGuide();
            ui.HideStopCountdown();
        }
    
    // this.enabled = false;
    ScheduleAnimalClear(animalDespawnDelay);
        // stop timer tracking and clear countdown
        zoneTimer?.StopTracking();
    }
}

        private void HandleTimerTimeout(CartController cart)
        {
            // Auto-resume cart after timeout
            if (cart != null)
            {
                cart.ResetSpeed();
                cart.AllowStop(false);
            }

            if (ui != null)
            {
                ui.SetZoneIconVisible(false);
                ui.HideGuide();
                ui.HideStopCountdown();
            }
            else
            {
                Debug.LogError("[SlowdownZone] UIManager not found; cannot hide zone icon after timeout.");
            }

                        // Schedule clearing animals after timeout (instead of immediate). This mirrors OnTriggerExit behavior.
                        ScheduleAnimalClear(animalDespawnDelay);
        }

        /// <summary>
        /// Called when a photo of an animal is captured while the cart is in this zone.
        /// Forwards to the timer so it can reset on *new* captures, then checks zone completion.
        /// </summary>
        public void NotifyAnimalCaptured(string animalName)
        {
            if (string.IsNullOrEmpty(animalName)) return;

            if (zoneTimer != null)
                zoneTimer.NotifyCaptured(animalName);

            CheckForZoneCompletion();
        }



    }
}
