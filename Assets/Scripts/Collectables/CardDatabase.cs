using System.Collections.Generic;
using UnityEngine;

namespace Collectables
{
    [CreateAssetMenu(fileName = "CardDatabase", menuName = "Collectables/Card Database")]
    public class CardDatabase : ScriptableObject
    {
        [SerializeField] private List<CardData> allCards;
        
        public List<CardData> AllCards => allCards;
    }
}
