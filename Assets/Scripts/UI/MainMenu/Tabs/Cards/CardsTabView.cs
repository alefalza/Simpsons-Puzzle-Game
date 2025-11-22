using Collectables;
using UnityEngine;

namespace UI.MainMenu.Tabs.Cards
{
    public class CardsTabView : TabView
    {
        [SerializeField] private CardDatabase database;
        [SerializeField] private Transform gridRoot;
        [SerializeField] private CardItemUI cardItemPrefab;

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
                item.Setup(card);
            }
        }
    }
}
