using UnityEngine;

namespace GameModes.BubbleMerge.UI
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererScroller : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 1f;

        private LineRenderer lineRenderer;
        private Material lineMat;
        private float offset;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            lineMat = Instantiate(lineRenderer.material);
            lineRenderer.material = lineMat;
        }

        private void Update()
        {
            offset += scrollSpeed * Time.deltaTime;
            lineMat.mainTextureOffset = new Vector2(offset, 0f);
        }
    }
}
