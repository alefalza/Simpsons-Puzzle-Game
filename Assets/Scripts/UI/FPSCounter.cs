using TMPro;
using UnityEngine;

namespace UI
{
    public sealed class FPSCounter : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text targetText;

        [Header("Format")]
        [SerializeField] private bool showMilliseconds = false;
        [SerializeField] private string prefix = "";

        [Header("Smoothing")]
        [SerializeField] private float updateInterval = 0.25f;
        [SerializeField] private float smoothing = 8f;

        private float smoothedDeltaTime = 1f / 60f;
        private float timer;

        private void Update()
        {
            float dt = Mathf.Max(0.000001f, Time.unscaledDeltaTime);
            smoothedDeltaTime = Mathf.Lerp(smoothedDeltaTime, dt, 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime));

            timer += Time.unscaledDeltaTime;
            if (timer < updateInterval) return;
            timer = 0f;

            float fps = 1f / smoothedDeltaTime;

            if (showMilliseconds)
            {
                float ms = smoothedDeltaTime * 1000f;
                targetText.text = $"{prefix}{fps:0} ({ms:0.0} ms)";
            }
            else
            {
                targetText.text = $"{prefix}{fps:0}";
            }
        }
    }
}
