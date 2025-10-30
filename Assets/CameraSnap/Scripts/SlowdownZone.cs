using UnityEngine;

namespace CameraSnap
{
    public class SlowdownZone : MonoBehaviour
    {
        [Header("Slowdown Settings")]
        public float slowedSpeed = 1f;
        public GameObject uiIcon;

        [Header("Animal Spawning")]
        public AnimalSpawner animalSpawner;

        
//Cart is reduced to slowed speed. The player is allowed to stop the cart. UI icon appears to indicate you need to press key
//or do movement to stop cart. Animals for the zone is spawned in..
        private void OnTriggerEnter(Collider other)
{
    CartController cart = other.GetComponentInParent<CartController>();
    if (cart != null)
    {
        cart.currentZone = this; 
        cart.SetSpeed(slowedSpeed);
        cart.AllowStop(true);
            if (uiIcon != null)
            {
                if (UIManager.Instance == null)
                {
                    Debug.LogError("[SlowdownZone] UIManager not found; cannot show zone icon.");
                }
                else
                {
                    UIManager.Instance.SetZoneIcon(uiIcon);
                    UIManager.Instance.SetZoneIconVisible(true);
                    // Show the guide squat animation when entering zone
                    UIManager.Instance.SetGuideState(UIManager.GuideState.Squat);
                }
            }
        if (animalSpawner != null) animalSpawner.SpawnAnimals();
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
            if (uiIcon != null)
            {
                if (UIManager.Instance == null)
                {
                    Debug.LogError("[SlowdownZone] UIManager not found; cannot hide zone icon.");
                }
                else
                {
                    UIManager.Instance.SetZoneIconVisible(false);
                    UIManager.Instance.HideGuide();
                }
            }
        if (animalSpawner != null) animalSpawner.ClearPreviousAnimals();
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
        if (uiIcon != null)
        {
            if (UIManager.Instance == null)
            {
                Debug.LogError("[SlowdownZone] UIManager not found; cannot hide zone icon or guide.");
            }
            else
            {
                UIManager.Instance.SetZoneIconVisible(false);
                UIManager.Instance.HideGuide();
            }
        }
//Deactivate zone and clear animals.
        this.enabled = false;
        animalSpawner.ClearPreviousAnimals();
    }
}



    }
}
