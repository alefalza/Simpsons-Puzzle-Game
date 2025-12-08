using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePopup : MonoBehaviour
{
    [Header("Popup Definition")]
    public PopupDefinition definition;

    protected CanvasGroup canvasGroup;
    protected bool isOpen = false;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (definition == null)
            Debug.LogError($"{name} missing PopupDefinition!");

        // Start invisible
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public virtual void Open()
    {
        isOpen = true;
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public virtual void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        StopAllCoroutines();
        StartCoroutine(FadeOut());
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
}
