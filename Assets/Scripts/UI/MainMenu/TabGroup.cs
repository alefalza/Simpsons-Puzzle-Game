using System.Collections.Generic;
using UnityEngine;

namespace UI.MainMenu
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private List<TabButton> tabButtons = new();

        private TabButton currentTab;

        private void Awake()
        {
            foreach (var tab in tabButtons)
                tab.SetGroup(this);
        }

        private void Start()
        {
            // Select first tab by default
            if (tabButtons.Count > 0)
                OnTabSelected(tabButtons[0]);
        }
        
        /// <summary>
        /// Called when a TabButton is selected.
        /// </summary>
        public void OnTabSelected(TabButton tab)
        {
            currentTab = tab;
            UpdateTabs();
        }

        /// <summary>
        /// Enables the view of the selected tab and disables the rest.
        /// </summary>
        private void UpdateTabs()
        {
            foreach (var tab in tabButtons)
            {
                if (tab == currentTab)
                    tab.targetView.Show();
                else
                    tab.targetView.Hide();
            }
        }
    }
}
