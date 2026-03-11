using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Constellation
{
    /// <summary>
    /// Attach to the Player GameObject.
    /// Shows cycling hint images based on proximity (2D top-down)
    /// with a juicy squash-and-stretch pop on each frame transition.
    ///
    /// Priority 1 – Holding a star near its destination → nearDestHints
    /// Priority 2 – Not holding, near an unplaced star  → nearStarHints
    /// Otherwise  – Everything hidden
    /// </summary>
    public class PlayerHintUI : MonoBehaviour
    {
        [Header("Near Star Hints (not holding anything)")]
        [SerializeField] private GameObject[] nearStarHints;
        [SerializeField] private float nearStarRange = 6f;

        [Header("Near Destination Hints (holding a star)")]
        [SerializeField] private GameObject[] nearDestHints;
        [SerializeField] private float nearDestRange = 6f;

        [Header("Cycle Settings")]
        [SerializeField] private float cycleInterval = 0.5f;
        [SerializeField] private bool showDebug = false;

        [Header("Squash & Stretch")]
        [Tooltip("How long the pop animation takes.")]
        [SerializeField] private float popDuration = 0.15f;

        [Tooltip("Multiplier for squash phase (wide & short).")]
        [SerializeField] private Vector2 squashMultiplier = new Vector2(1.3f, 0.7f);

        [Tooltip("Multiplier for stretch phase (tall & narrow).")]
        [SerializeField] private Vector2 stretchMultiplier = new Vector2(0.85f, 1.2f);

        // ── internal ───────────────────────────────────────────────────
        private PlayerController playerCont;

        private GameObject[] activeHints;
        private int cycleIndex;
        private float cycleTimer;

        private Coroutine popRoutine;

        // cache each hint's original scale so we never stomp editor values
        private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

        // ── lifecycle ──────────────────────────────────────────────────

        void Start()
        {
            playerCont = GetComponent<PlayerController>();
            if (playerCont == null)
                Debug.LogError("[PlayerHintUI] No PlayerController found on " + gameObject.name);

            CacheScales(nearStarHints);
            CacheScales(nearDestHints);

            HideAll(nearStarHints);
            HideAll(nearDestHints);
        }

        void Update()
        {
            if (playerCont == null) return;

            GameObject[] desired = GetDesiredHints();

            if (desired != activeHints)
            {
                StopPop();
                HideAll(activeHints);
                activeHints = desired;
                cycleIndex = 0;
                cycleTimer = 0f;
                ShowOnly(activeHints, 0);
                Pop(activeHints, 0);
            }

            if (activeHints != null && activeHints.Length > 1)
            {
                cycleTimer += Time.deltaTime;
                if (cycleTimer >= cycleInterval)
                {
                    cycleTimer -= cycleInterval;
                    cycleIndex = (cycleIndex + 1) % activeHints.Length;
                    ShowOnly(activeHints, cycleIndex);
                    Pop(activeHints, cycleIndex);
                }
            }
        }

        // ── decision logic ─────────────────────────────────────────────

        private GameObject[] GetDesiredHints()
        {
            if (playerCont.grabedStar != null)
            {
                StarScript held = playerCont.grabedStar.GetComponent<StarScript>();

                if (held != null && held.destination != null)
                {
                    float dist = Dist2D(transform.position, held.destination.transform.position);

                    if (showDebug)
                        Debug.Log($"[HintUI] Holding star, dist to dest: {dist:F1} / threshold: {nearDestRange}");

                    if (dist <= nearDestRange)
                        return nearDestHints;
                }

                return null;
            }

            StarScript[] stars = FindObjectsOfType<StarScript>(false);

            foreach (StarScript star in stars)
            {
                if (star == null) continue;
                if (star.foundHome) continue;

                float dist = Dist2D(transform.position, star.transform.position);

                if (showDebug && dist <= nearStarRange * 2f)
                    Debug.Log($"[HintUI] Star '{star.name}' dist: {dist:F1} / threshold: {nearStarRange} / foundHome: {star.foundHome}");

                if (dist <= nearStarRange)
                    return nearStarHints;
            }

            return null;
        }

        // ── squash & stretch ───────────────────────────────────────────

        private void Pop(GameObject[] hints, int index)
        {
            if (hints == null || hints.Length == 0) return;
            GameObject target = hints[index];
            if (target == null) return;

            StopPop();
            popRoutine = StartCoroutine(PopCoroutine(target.transform));
        }

        private void StopPop()
        {
            if (popRoutine != null)
            {
                StopCoroutine(popRoutine);
                popRoutine = null;
            }
        }

        /// <summary>
        /// Three-phase tween: squash → stretch → settle.
        /// All scales are relative to the object's original editor scale.
        /// </summary>
        private IEnumerator PopCoroutine(Transform target)
        {
            Vector3 baseScale = GetOriginalScale(target.gameObject);

            Vector3 squash = new Vector3(
                baseScale.x * squashMultiplier.x,
                baseScale.y * squashMultiplier.y,
                baseScale.z);

            Vector3 stretch = new Vector3(
                baseScale.x * stretchMultiplier.x,
                baseScale.y * stretchMultiplier.y,
                baseScale.z);

            float third = popDuration / 3f;

            // phase 1: squash
            yield return TweenScale(target, baseScale, squash, third);

            // phase 2: stretch
            yield return TweenScale(target, squash, stretch, third);

            // phase 3: settle back to original
            yield return TweenScale(target, stretch, baseScale, third);

            target.localScale = baseScale;
            popRoutine = null;
        }

        private IEnumerator TweenScale(Transform target, Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                t = t * t * (3f - 2f * t); // smoothstep
                target.localScale = Vector3.LerpUnclamped(from, to, t);
                yield return null;
            }
            target.localScale = to;
        }

        // ── helpers ────────────────────────────────────────────────────

        private void CacheScales(GameObject[] hints)
        {
            if (hints == null) return;
            foreach (GameObject obj in hints)
            {
                if (obj != null && !originalScales.ContainsKey(obj))
                    originalScales[obj] = obj.transform.localScale;
            }
        }

        private Vector3 GetOriginalScale(GameObject obj)
        {
            if (originalScales.TryGetValue(obj, out Vector3 scale))
                return scale;
            return obj.transform.localScale;
        }

        private float Dist2D(Vector3 a, Vector3 b)
        {
            return Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x, b.y));
        }

        private void ShowOnly(GameObject[] hints, int index)
        {
            if (hints == null) return;
            for (int i = 0; i < hints.Length; i++)
            {
                if (hints[i] != null)
                    hints[i].SetActive(i == index);
            }
        }

        private void HideAll(GameObject[] hints)
        {
            if (hints == null) return;
            foreach (GameObject obj in hints)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    obj.transform.localScale = GetOriginalScale(obj);
                }
            }
        }

        // ── scene view debug ───────────────────────────────────────────

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Vector3 pos = transform.position;
            pos.z = 0f;

            Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
            Gizmos.DrawWireSphere(pos, nearStarRange);

            Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);
            Gizmos.DrawWireSphere(pos, nearDestRange);
        }
#endif
    }
}