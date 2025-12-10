using System;
using UnityEngine;
using System.Collections;

public enum Priority
{
    Urgent = 4,
    High = 3,
    Medium = 2,
    Low = 1
}

public abstract class PopupData
{
    public Priority Priority { get; private set; }

    protected PopupData(Priority priority)
    {
        Priority = priority;
    }
}

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

    protected virtual void Start()
    {
        
    }

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
        {
            CheckDestroy();
            InvokeOnClosed();
        }
        else
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        IsFading = true;
        
        float t = 0f;
        float duration = Definition.fadeInDuration;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / duration;
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

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / duration);
            yield return null;
        }

        canvasGroup.alpha = 0;

        InvokeOnClosed();

        IsFading = false;

        CheckDestroy();
    }

    protected void CheckDestroy()
    {
        if (Definition.destroyOnClose)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    protected void InvokeOnClosed()
    {
        OnClosed?.Invoke(Definition.destroyOnClose);
    }

    protected virtual void OnDestroy()
    {
        
    }
}
