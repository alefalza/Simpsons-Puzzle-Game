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
        [SerializeField] private Image image;
        
        public DonutColor Color { get; private set; }

        private void Start()
        {
            image.color = UnityEngine.Color.white;
        }

        public void Initialize(DonutColor color, Sprite sprite)
        {
            Color = color;
            
            if (sprite != null)
            {
                image.sprite = sprite;
            }
        }
    }
}
