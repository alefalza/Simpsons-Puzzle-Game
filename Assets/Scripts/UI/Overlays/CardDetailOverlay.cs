using System.Collections;
using Collectables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Overlays
{
    public class CardDetailOverlay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform panel;
        [SerializeField] private TMP_Text cardName;
        [SerializeField] private Image cardIcon;
        [SerializeField] private TMP_Text cardDescription;
        [SerializeField] private Button closeButton;

        [Header("Animation Settings")]
        [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private float startScale = 0.6f;

        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            canvasGroup.alpha = 0f;
        }

        public void Show(CardData data)
        {
            // Set UI Data
            cardName.text = data.CardName;
            cardIcon.sprite = data.CardImage;
            cardDescription.text = data.CardDescription;

            // Reset animation state
            panel.localScale = Vector3.one * startScale;
            canvasGroup.alpha = 0f;

            gameObject.SetActive(true);

            StartCoroutine(AnimateIn());
        }

        private IEnumerator AnimateIn()
        {
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;

                // Fade
                canvasGroup.alpha = Mathf.Lerp(0, 1, t);

                // Scale with easing
                float scaleT = EaseOutBack(t);
                panel.localScale = Vector3.one * Mathf.Lerp(startScale, 1f, scaleT);

                yield return null;
            }

            canvasGroup.alpha = 1f;
            panel.localScale = Vector3.one;
        }

        public void Close()
        {
            StartCoroutine(AnimateOut());
        }

        private IEnumerator AnimateOut()
        {
            float t = 0f;
            Vector3 originalScale = panel.localScale;

            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;

                // Fade
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

                // Shrink
                panel.localScale = Vector3.Lerp(originalScale, originalScale * 0.6f, t);

                yield return null;
            }

            gameObject.SetActive(false);
        }

        private float EaseOutBack(float t)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
        }
    }
}
