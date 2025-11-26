using UnityEngine;

namespace GameModes.BubbleMerge.UI
{
    public class LineRendererScroller : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float scrollSpeed = 1f;

        private Material lineMat;
        private float offset;

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
