using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public abstract class PopupData
{
    public Priority Priority { get; private set; }
    public string Message { get; private set; }
    public string Title { get; private set; }
    
    public PopupData(Priority priority, string message, string title)
    {
        Priority = priority;
        Message = message;
        Title = title;
    }
}

[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePopup : MonoBehaviour
{
    [Header("Popup Definition")]
    public PopupDefinition definition;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button closeButton;

    protected CanvasGroup canvasGroup;
    protected bool isOpen = false;

    public event Action OnOpened;
    public event Action OnClosed;
    public event Action OnHide;

    protected PopupData popupData;
    
    protected virtual void Awake()
    {
        /*
        canvasGroup = GetComponent<CanvasGroup>();

        if (definition == null)
            Debug.LogError($"{name} missing PopupDefinition!");

        // Start invisible
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        */
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    public virtual void Setup(PopupData data)
    {
        popupData = data;
    }

    public virtual void Open()
    {
        //isOpen = true;
        //StopAllCoroutines();
        //StartCoroutine(FadeIn());
        titleText.text = popupData.Title;
        gameObject.SetActive(true);
        OnOpened?.Invoke();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        OnHide?.Invoke();
    }

    public virtual void Close()
    {
        //if (!isOpen) return;
        //isOpen = false;

        OnClosed?.Invoke();
        Destroy(gameObject);
        //StopAllCoroutines();
        //StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float duration = definition.overrideFadeDuration ? definition.fadeInDuration : 0.15f;
        float t = 0;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = definition.blocksInput;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / duration;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        float duration = definition.overrideFadeDuration ? definition.fadeOutDuration : 0.15f;
        float t = 0;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - (t / duration);
            yield return null;
        }

        canvasGroup.alpha = 0;

        if (definition.destroyOnClose)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
    
    protected void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }
}
