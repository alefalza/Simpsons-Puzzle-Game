using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameModes.DonutStack.Core
{
    [Serializable]
    public enum PieceColor
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
    public class Piece : MonoBehaviour
    {
        private Image image;
    
        private static readonly Color[] colorMap = new Color[]
        {
            UnityEngine.Color.clear,
            new Color(1f, 0.2f, 0.2f),
            new Color(0.2f, 0.4f, 1f),
            new Color(0.3f, 1f, 0.3f),
            new Color(1f, 1f, 0.2f),
            new Color(0.95f, 0.95f, 0.95f),
            new Color(0.5f, 0.5f, 0.5f),
            new Color(1f, 0.3f, 1f)
        };
    
        public PieceColor Color { get; private set; }

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void Initialize(PieceColor color)
        {
            Color = color;
            image.color = colorMap[(int)color];
        }
    }
}
