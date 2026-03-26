using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected bool isDragging;
    protected Vector2 originalPosition;
    protected Transform originalParent;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        
        originalPosition = transform.position;
        originalParent = transform.parent;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    protected T DetectObjectUnderPointer<T>(PointerEventData eventData) where T : Component
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (var r in results)
        {
            if (r.gameObject.TryGetComponent<T>(out var component))
                return component;
        }

        var worldPos = Camera.main != null ?
            Camera.main.ScreenToWorldPoint(eventData.position) : (Vector3)eventData.position;
        worldPos.z = 0;
        
        var hit = Physics2D.OverlapPoint(worldPos);
        
        if (hit != null)
        {
            return hit.GetComponent<T>();
        }

        return null;
    }
}
