using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModes.DonutStack.Gameplay
{
    public class GridCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image cellImage;
        [SerializeField] private Color normalColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.5f, 0.7f);

        public Vector2Int AxialCoords { get; private set; }
        public Core.DonutStack Stack { get; private set; }
        
        public bool IsOccupied => Stack != null;

        private void Awake()
        {
            cellImage.color = normalColor;
        }

        public void Initialize(Vector2Int coords)
        {
            AxialCoords = coords;
        }
        
        public void SetStack(Core.DonutStack newStack)
        {
            Stack = newStack;
        }

        public void ClearStack()
        {
            Stack = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsOccupied)
                cellImage.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            cellImage.color = normalColor;
        }
    }
}
