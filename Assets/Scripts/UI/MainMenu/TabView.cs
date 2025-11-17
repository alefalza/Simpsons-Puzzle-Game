using UnityEngine;

namespace UI.MainMenu
{
    public class TabView : MonoBehaviour
    {
        /// <summary>
        /// Enables this view.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Disables this view.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
