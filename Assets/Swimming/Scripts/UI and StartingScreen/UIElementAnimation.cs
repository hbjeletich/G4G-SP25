using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swimming
{
    public class UIElementAnimation : MonoBehaviour
    {
        [SerializeField] private Vector3 moveAmount = new Vector3(20f, 0f, 0f); // How much to move (x,y,z)
        [SerializeField] private float animationDuration = 0.5f; // How long the movement animation takes
        [SerializeField] private float displayDuration = 5f; // Total time to display
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Vector3 startPosition;

        private void OnEnable()
        {
            startPosition = transform.localPosition;

            StartCoroutine(AnimateElement());
        }

        private IEnumerator AnimateElement()
        {
            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                float t = animationCurve.Evaluate(elapsed / animationDuration);
                transform.localPosition = startPosition + moveAmount * t;
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = startPosition + moveAmount;
        }
    }
}