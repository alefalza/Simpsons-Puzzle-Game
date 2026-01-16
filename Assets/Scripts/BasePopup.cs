using System;
using UnityEngine;
using System.Collections;

namespace Core.Services.PopupService
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BasePopup : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;

        public PopupData PopupData { get; private set; }
        public PopupDefinition Definition { get; private set; }

        public event Action OnOpened;
        public event Action<bool> OnClosed;

        public bool IsFading { get; private set; }

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        protected virtual void Start() { }

        public virtual void Setup(PopupData data, PopupDefinition def)
        {
            PopupData = data;
            Definition = def;
        }

        public virtual void Open()
        {
            StartCoroutine(FadeIn());
        }

        public virtual void Close(bool immediate = false)
        {
            if (immediate)
                CheckDestroyAndInvokeOnClosed();
            else
                StartCoroutine(FadeOut());
        }

        private IEnumerator FadeIn()
        {
            IsFading = true;
            
            float t = 0f;
            float duration = Definition.fadeInDuration;
            float inverseDuration = 1f / duration;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = t * inverseDuration;
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            OnOpened?.Invoke();

            IsFading = false;
        }

        private IEnumerator FadeOut()
        {
            IsFading = true;
            
            float t = 0f;
            float duration = Definition.fadeOutDuration;
            float inverseDuration = 1f / duration;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = 1f - (t * inverseDuration);
                yield return null;
            }

            canvasGroup.alpha = 0;
            
            CheckDestroyAndInvokeOnClosed();
            
            IsFading = false;
        }

        protected void CheckDestroyAndInvokeOnClosed()
        {
            bool destroyOnClose = Definition.destroyOnClose;
            
            if (destroyOnClose)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
            
            OnClosed?.Invoke(destroyOnClose);
        }

        protected virtual void OnDestroy() { }
    }
}
