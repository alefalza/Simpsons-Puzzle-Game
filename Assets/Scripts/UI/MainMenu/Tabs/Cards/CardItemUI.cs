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

        public void Setup(CardData cardData)
        {
            data = cardData;
            title.text = cardData.CardName;
            //icon.sprite = cardData.CardImage;
            
            bool unlocked = cardData.Unlocked;

            lockedOverlay.SetActive(!unlocked);
            button.interactable = unlocked;
        }
        
        public void OnClicked()
        {
            if (!data.Unlocked)
                return;

            Debug.Log($"Clicked {data.CardName}");
            // Abrir detalle, o lo que quieras
        }
    }
}
