using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PopupItem
{
    public string Type;
    public BasePopup Popup;
}

[CreateAssetMenu]
public class PopupsLibrary : ScriptableObject
{
    public List<PopupItem> PopupItems;
}
