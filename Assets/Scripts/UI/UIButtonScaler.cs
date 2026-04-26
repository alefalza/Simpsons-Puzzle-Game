using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Scale Settings")]
    [SerializeField] private Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);
    [SerializeField] private float duration = 0.1f;

    private Vector3 originalScale;
    private Coroutine scaleRoutine;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartScaleRoutine(pressedScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartScaleRoutine(originalScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaleRoutine(originalScale);
    }

    private void StartScaleRoutine(Vector3 targetScale)
    {
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
        }

        scaleRoutine = StartCoroutine(AnimateScale(targetScale));
    }

    private IEnumerator AnimateScale(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration; // use unscaled time so it works in paused menus
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localScale = target;
        scaleRoutine = null;
    }
}
