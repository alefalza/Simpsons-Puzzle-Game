using UnityEngine;

namespace UI
{
    public class FollowWorldTargetUI : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private Transform target;

        private RectTransform rectTransform;
        private Canvas canvas;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
        }

        private void LateUpdate()
        {
            Vector3 screenPos = worldCamera.WorldToScreenPoint(target.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : worldCamera,
                out Vector2 localPos
            );

            rectTransform.anchoredPosition = localPos;
        }
    }
}
