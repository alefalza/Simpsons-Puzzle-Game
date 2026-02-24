using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameModes.DonutStack.Core
{
    [Serializable]
    public enum DonutColor
    {
        None,
        Red,
        Blue,
        Green,
        Yellow,
        White,
        Gray,
        Magenta
    }

    [RequireComponent(typeof(Image))]
    public class Donut : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] colorSprites;
        
        private Image image;
        
        public DonutColor Color { get; private set; }

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void Start()
        {
            image.color = UnityEngine.Color.white;
        }

        public void Initialize(DonutColor color)
        {
            Color = color;

            int index = (int)color;
            
            if (colorSprites != null && index >= 0 && index < colorSprites.Length)
            {
                image.sprite = colorSprites[index];
            }
            else
            {
                image.sprite = null;
            }
        }
    }
}
