using GameModes.BubbleMerge.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GameModes.BubbleMerge.UI
{
    public class BubbleIconUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;

        private Vector3 baseScale;

        private void Awake()
        {
            baseScale = iconImage.rectTransform.localScale;
        }

        /// <summary>
        /// Assigns the icon sprite. If a Bubble is provided, scales the icon to match its scale.
        /// </summary>
        public void SetBubbleIcon(Sprite icon, Bubble bubble = null)
        {
            iconImage.sprite = icon;

            if (bubble != null)
            {
                iconImage.SetNativeSize();
                iconImage.rectTransform.localScale = baseScale;
        
                float bubbleScale = bubble.transform.localScale.x;
                iconImage.rectTransform.localScale *= bubbleScale;
            }
        }
    }
}
