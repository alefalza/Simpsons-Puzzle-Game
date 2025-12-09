using System.Linq;
using UnityEngine;

public interface IPopupFactory
{
    /// <summary>
    /// Create a new instance of APopup based on input data.
    /// </summary>
    /// <param name="popupData">Popup data.</param>
    /// <param name="parent">Popup parent Transform.</param>
    /// <returns>Returns new <see cref="BasePopup"/></returns>
    BasePopup InstantiatePopup(PopupData popupData, Transform parent);
}

public class PopupFactory : IPopupFactory
{
    private readonly PopupsLibrary popupsLibrary;

    public PopupFactory(PopupsLibrary popupsLibrary)
    {
        this.popupsLibrary = popupsLibrary;
    }

    public BasePopup InstantiatePopup(PopupData popupData, Transform parent)
    {
        if (popupData == null || parent == null)
        {
            Debug.LogError($"[{nameof(PopupFactory)}] - Unable to instantiate popup. Null data");
            return null;
        }
            
        var data = popupsLibrary.PopupItems.FirstOrDefault(t => t.Type.Equals(popupData.GetType().Name));
        
        if (data == null)
        {
            Debug.LogError($"[{nameof(PopupFactory)}] - Unable to instantiate popup. Can't find {nameof(PopupData)} popup");
            return null;
        }

        if (data.Popup == null)
        {
            Debug.LogError($"[{nameof(PopupFactory)}] - Unable to instantiate popup. {nameof(PopupData)} popup is null");
            return null;
        }

        var popup = Object.Instantiate(data.Popup, parent);
        popup.Setup(popupData);
        
        return popup;
    }
}
