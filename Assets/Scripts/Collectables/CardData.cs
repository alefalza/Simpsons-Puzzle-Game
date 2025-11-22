using UnityEngine;

namespace Collectables
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "Collectables/Card", order = 0)]
    public class CardData : ScriptableObject
    {
        [SerializeField] private string cardName;
        [SerializeField] private Sprite cardImage;
        [SerializeField] private bool unlocked;

        public string CardName => cardName;
        public Sprite CardImage => cardImage;
        public bool Unlocked => unlocked;
    }
}
