using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class TabButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TabView targetView;
        [SerializeField] private MainMenuTab tabId;

        private TabGroup tabGroup;

        public TabView TargetView => targetView;
        public MainMenuTab TabId => tabId;

        /// <summary>
        /// Sets the TabGroup this button belongs to.
        /// </summary>
        public void SetGroup(TabGroup group)
        {
            tabGroup = group;
            button.onClick.AddListener(OnClick);
        }

        /// <summary>
        /// Called when the button is pressed.
        /// </summary>
        private void OnClick()
        {
            tabGroup.OnTabButtonClicked(this);
        }
    }
}
