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

        

        private void OnTriggerEnter(Collider other)
{
    CartController cart = other.GetComponentInParent<CartController>();
    if (cart != null)
    {
        cart.currentZone = this; 
        cart.SetSpeed(slowedSpeed);
        cart.AllowStop(true);
        if (uiIcon != null) uiIcon.SetActive(true);
        if (animalSpawner != null) animalSpawner.SpawnAnimals();
    }
}

private void OnTriggerExit(Collider other)
{
    CartController cart = other.GetComponentInParent<CartController>();
    if (cart != null)
    {
        if (cart.currentZone == this) cart.currentZone = null; // clear it
        cart.ResetSpeed();
        cart.AllowStop(false);
        if (uiIcon != null) uiIcon.SetActive(false);
        if (animalSpawner != null) animalSpawner.ClearPreviousAnimals();
    }
}

        public void CheckForZoneCompletion()
{
    if (animalSpawner == null || GameManager.Instance == null)
        return;

    var animals = animalSpawner.GetSpawnedAnimals();
    if (animals == null || animals.Count == 0)
        return;

    int capturedCount = 0;

    foreach (var a in animals)
    {
        if (a == null) continue;
        var behavior = a.GetComponent<AnimalBehavior>();
if (behavior != null && GameManager.Instance.HasCaptured(behavior.animalData.animalName))
    capturedCount++;

    }

    if (capturedCount >= animals.Count)
    {
        Debug.Log("[SlowdownZone] All zone animals (2) captured! Resuming cart...");

        CartController cart = FindObjectOfType<CartController>();
        if (cart != null)
        {
            cart.ResetSpeed();    // back to normal
            cart.AllowStop(false);
        }

        if (uiIcon != null)
            uiIcon.SetActive(false);

        this.enabled = false;
        animalSpawner.ClearPreviousAnimals();
    }
}



    }
}
