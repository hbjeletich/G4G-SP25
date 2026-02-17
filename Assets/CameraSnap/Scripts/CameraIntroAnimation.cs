using UnityEngine;

namespace CameraSnap
{
    public class CameraIntroAnimation : MonoBehaviour
    {
        [Header("Intro Lerp")]
        public float startXRotation = 30f;
        public float targetXRotation = 0f;

        public float introDuration = 2f;
        public AnimationCurve introCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Idle Bounce")]
        public float bounceAmplitude = 1.5f;
        public float bounceSpeed = 0.6f;
        public float bounceDelay = 0.3f;

        private float elapsedTime = 0f;
        private bool introComplete = false;
        private float bounceTimer = 0f;
        private float bounceDelayTimer = 0f;
        private bool bounceStarted = false;

        private void Start()
        {
            // Snap to start rotation immediately
            SetXRotation(startXRotation);
        }

        private void Update()
        {
            if (!introComplete)
            {
                RunIntro();
            }
            else
            {
                RunIdleBounce();
            }
        }

        private void RunIntro()
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / introDuration);
            float curved = introCurve.Evaluate(t);
            float xRot = Mathf.LerpAngle(startXRotation, targetXRotation, curved);
            SetXRotation(xRot);

            if (t >= 1f)
            {
                introComplete = true;
                SetXRotation(targetXRotation);
            }
        }

        private void RunIdleBounce()
        {
            // Small delay before bounce kicks in so it doesn't feel abrupt
            if (!bounceStarted)
            {
                bounceDelayTimer += Time.deltaTime;
                if (bounceDelayTimer >= bounceDelay)
                    bounceStarted = true;
                return;
            }

            bounceTimer += Time.deltaTime;

            // Sine wave bounce on top of the target rotation
            float bounce = Mathf.Sin(bounceTimer * bounceSpeed * Mathf.PI * 2f) * bounceAmplitude;
            SetXRotation(targetXRotation + bounce);
        }

        private void SetXRotation(float xDegrees)
        {
            Vector3 euler = transform.eulerAngles;
            euler.x = xDegrees;
            transform.eulerAngles = euler;
        }

        public void Restart()
        {
            elapsedTime = 0f;
            introComplete = false;
            bounceTimer = 0f;
            bounceDelayTimer = 0f;
            bounceStarted = false;
            SetXRotation(startXRotation);
        }
    }
}