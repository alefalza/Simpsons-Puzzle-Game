using UnityEngine;

namespace UI.MainMenu
{
    public class TabView : MonoBehaviour, ITabView
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
