using UnityEngine;
using System.Collections.Generic;

public class ShimmerHum : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;                     // drag your player
    [SerializeField] private List<GameObject> stars = new();       // drag star GameObjects

    [Header("Audio")]
    [SerializeField] private AudioSource humSource;                // one audio source only
    [SerializeField] private float maxVolume = 0.6f;
    [SerializeField] private float fadeSpeed = 3f;
    [SerializeField] private float range = 6f;
    [SerializeField] private float carryDistanceThreshold = 0.5f; // distance to detect carried stars

    private GameObject currentClosestStar;

    private void Start()
    {
        if (humSource != null)
        {
            humSource.loop = true;
            humSource.volume = 0f;
            humSource.Play(); // ensures hum starts
        }
    }

    private void Update()
    {
        if (player == null || stars.Count == 0) return;

        float bestDist = Mathf.Infinity;
        GameObject bestStar = null;

        foreach (var star in stars)
        {
            if (star == null) continue; // removed / destroyed

            var starScript = star.GetComponent<Constellation.StarScript>();
            if (starScript == null) continue;

            // skip stars at destination
            if (starScript.foundHome) continue;

            // skip stars that are being carried (position-based detection)
            float distanceToPlayer = Vector2.Distance(player.position, star.transform.position);
            if (distanceToPlayer < carryDistanceThreshold) continue;

            // closest star logic
            if (distanceToPlayer < bestDist)
            {
                bestDist = distanceToPlayer;
                bestStar = star;
            }
        }

        currentClosestStar = bestStar;

        float targetVolume = 0f;

        if (currentClosestStar != null)
        {
            float t = Mathf.Clamp01(1f - (bestDist / range));
            targetVolume = t * maxVolume;
        }

        // smooth fade interpolation
        if (humSource != null)
        {
            humSource.volume = Mathf.MoveTowards(
                humSource.volume,
                targetVolume,
                fadeSpeed * Time.deltaTime
            );
        }
    }

    // call this from your star pickup or place logic
    public void OnStarCollected(GameObject star)
    {
        stars.Remove(star);
    }
}
