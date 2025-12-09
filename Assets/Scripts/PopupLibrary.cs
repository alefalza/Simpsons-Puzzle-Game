using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PopupItem
{
    public string name;
    public BasePopup prefab;
}

[CreateAssetMenu(fileName = "PopupsLibrary", menuName = "Core/Popups Library")]
public class PopupLibrary : ScriptableObject
{
    [SerializeField] private List<PopupItem> popupItems;
    
    public List<PopupItem> PopupItems => popupItems;
}
