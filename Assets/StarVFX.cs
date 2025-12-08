using UnityEngine;

namespace Constellation
{
    public class StarParticlePlayer : MonoBehaviour
    {
        [Header("Star References")]
        [Tooltip("Drag all star GameObjects here")]
        public GameObject[] stars;

        [Header("Particle Settings")]
        [Tooltip("The particle effect prefab to spawn when any star finds its home")]
        public GameObject particleEffectPrefab;

        private bool[] previousFoundHome;

        void Start()
        {
            previousFoundHome = new bool[stars.Length];
            
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    StarScript starScript = stars[i].GetComponent<StarScript>();
                    if (starScript != null)
                    {
                        previousFoundHome[i] = starScript.foundHome;
                    }
                }
            }
        }

        void Update()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    StarScript starScript = stars[i].GetComponent<StarScript>();
                    if (starScript != null)
                    {
                        if (starScript.foundHome && !previousFoundHome[i])
                        {
                            SpawnParticleEffect(starScript.destination.transform.position);
                        }
                        previousFoundHome[i] = starScript.foundHome;
                    }
                }
            }
        }

        void SpawnParticleEffect(Vector3 position)
        {
            if (particleEffectPrefab != null)
            {
                GameObject effect = Instantiate(particleEffectPrefab, position, Quaternion.identity);
                
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
                }
                else
                {
                    Destroy(effect, 3f);
                }
            }
        }
    }
}