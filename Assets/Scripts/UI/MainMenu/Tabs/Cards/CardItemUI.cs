using System;
using Collectables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.Tabs.Cards
{
    public class CardItemUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text title;
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private Button button;

        private CardData data;
        private Action<CardData> onClicked;

        public void Setup(CardData cardData, Action<CardData> onClick)
        {
            data = cardData;
            onClicked = onClick;
            
            title.text = cardData.CardName;

            if (cardData.CardImage != null)
            {
                icon.sprite = cardData.CardImage;
            }
            
            bool unlocked = cardData.Unlocked;
            lockedOverlay.SetActive(!unlocked);
            button.interactable = unlocked;
        }
        
        public void OnClicked()
        {
            if (!data.Unlocked)
                return;

            onClicked?.Invoke(data);
        }
    }
}
