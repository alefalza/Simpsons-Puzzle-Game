using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class TabButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TabView targetView;

        private TabGroup tabGroup;

        public TabView TargetView => targetView;

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
