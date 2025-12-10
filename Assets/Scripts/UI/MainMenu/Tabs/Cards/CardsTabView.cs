using Collectables;
using Core;
using Core.Services.PopupService;
using UI.Popups;
using UnityEngine;

namespace UI.MainMenu.Tabs.Cards
{
    public class CardsTabView : TabView
    {
        [SerializeField] private CardDatabase database;
        [SerializeField] private Transform gridRoot;
        [SerializeField] private CardItemUI cardItemPrefab;
        [SerializeField] private PopupDefinition cardDetailPopupDefinition;

        private IPopupService popupService;
        
        private void Awake()
        {
            popupService = ServiceLocator.Get<IPopupService>();
        }

        private void Start()
        {
            Populate();
        }

        private void Populate()
        {
            foreach (Transform child in gridRoot)
                Destroy(child.gameObject);

            foreach (var card in database.AllCards)
            {
                var item = Instantiate(cardItemPrefab, gridRoot);
                item.Setup(card, OnCardClickedCallback);
            }
        }

        private void OnCardClickedCallback(CardData cardData)
        {
            popupService.Push(cardDetailPopupDefinition, new CardDetailPopupData(Priority.Low, cardData));
        }
    }
}
