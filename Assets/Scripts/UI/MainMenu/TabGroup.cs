using System.Collections.Generic;
using Core;
using Core.Services.UIService;
using UnityEngine;

namespace UI.MainMenu
{
    public enum MainMenuTab
    {
        None = 0,
        Home = 1,
        Cards = 2,
        Play = 3,
        Shop = 4,
        Settings = 5
    }
    
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private List<TabButton> tabButtons = new();

        private TabButton currentTabButton;
        private IUIService uiService;

        private void Awake()
        {
            uiService = ServiceLocator.Get<IUIService>();
            
            foreach (var tabButton in tabButtons)
                tabButton.SetGroup(this);
        }

        private void Start()
        {
            if (tabButtons.Count == 0)
                return;

            var targetTab = uiService.LastTabSeen;
            var tabButton = tabButtons.Find(t => t.TabId == targetTab) ?? tabButtons[0];

            OnTabButtonClicked(tabButton);
        }
        
        /// <summary>
        /// Called when a TabButton is selected.
        /// </summary>
        public void OnTabButtonClicked(TabButton tabButton)
        {
            currentTabButton = tabButton;
            uiService.SetLastTabSeen(tabButton.TabId);
            UpdateTabViews();
        }

        /// <summary>
        /// Enables the view of the selected tab and disables the rest.
        /// </summary>
        private void UpdateTabViews()
        {
            foreach (var tabButton in tabButtons)
            {
                if (tabButton == currentTabButton)
                    tabButton.TargetView.Show();
                else
                    tabButton.TargetView.Hide();
            }
        }
    }
}
