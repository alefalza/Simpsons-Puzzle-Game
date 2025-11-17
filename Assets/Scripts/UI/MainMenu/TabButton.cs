using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class TabButton : MonoBehaviour
    {
        [Header("Assigned in Inspector")]
        public Button button;
        public TabView targetView;

        private TabGroup tabGroup;

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
            tabGroup.OnTabSelected(this);
        }
    }
}
