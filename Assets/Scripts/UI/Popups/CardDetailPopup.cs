using System.Collections;
using Collectables;
using Core.Services.PopupService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class CardDetailPopup : BasePopup
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform panel;
        [SerializeField] private TMP_Text cardName;
        [SerializeField] private Image cardIcon;
        [SerializeField] private TMP_Text cardDescription;
        [SerializeField] private Button closeButton;

        [Header("Animation Settings")]
        [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private float initialScale = 0.6f;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(() => Close());
        }

        public override void Open()
        {
            CardData cardData = (PopupData as CardDetailPopupData)?.CardData;

            if (cardData == null) return;
            
            SetUIData(cardData);
            StartCoroutine(ScaleIn());
        }
        
        public override void Close(bool immediate = false)
        {
            if (immediate)
                CheckDestroyAndInvokeOnClosed();
            else
                StartCoroutine(ScaleOut());
        }

        private void SetUIData(CardData cardData)
        {
            cardName.text = cardData.CardName;
            cardIcon.sprite = cardData.CardImage;
            cardDescription.text = cardData.CardDescription;
        }
        
        private IEnumerator ScaleIn()
        {
            panel.localScale = Vector3.one * initialScale;
            canvasGroup.alpha = 0f;
            
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;

                // Fade
                canvasGroup.alpha = Mathf.Lerp(0, 1, t);

                // Scale with easing
                float scaleT = EaseOutBack(t);
                panel.localScale = Vector3.one * Mathf.Lerp(initialScale, 1f, scaleT);

                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            panel.localScale = Vector3.one;
        }

        private IEnumerator ScaleOut()
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
            
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            CheckDestroyAndInvokeOnClosed();
        }

        private float EaseOutBack(float t)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
        }

        protected override void OnDestroy()
        {
            closeButton.onClick.RemoveListener(() => Close());
            base.OnDestroy();
        }
    }

    public class CardDetailPopupData : PopupData
    {
        public CardData CardData { get; private set; }

        public CardDetailPopupData(Priority priority, CardData cardData) : base(priority)
        {
            CardData = cardData;
        }
    }
}
