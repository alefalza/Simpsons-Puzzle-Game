using UnityEngine;

public class CanvasScaleSync : MonoBehaviour
{
    public RectTransform hudCanvas;
    public RectTransform worldCanvas;

    private void Awake()
    {
        if (worldCanvas == null)
            worldCanvas = GetComponent<RectTransform>();
    }
    
    private void Start()
    {
        SyncScale();
    }

    private void Update()
    {
        if (worldCanvas.localScale != hudCanvas.localScale)
        {
            SyncScale();
        }
    }

    private void SyncScale()
    {
        if (hudCanvas != null && worldCanvas != null)
        {
            worldCanvas.localScale = hudCanvas.localScale;
        }
    }
}
