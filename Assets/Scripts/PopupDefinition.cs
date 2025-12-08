using UnityEngine;

[CreateAssetMenu(fileName = "PopupDefinition", menuName = "Popups/Popup Definition")]
public class PopupDefinition : ScriptableObject
{
    public string popupId;

    [Header("Prefab")]
    public BasePopup prefab;

    [Header("Behavior")]
    public bool blocksInput = true;
    public bool destroyOnClose = true;

    [Header("Override Animations")]
    public bool overrideFadeDuration = false;
    public float fadeInDuration = 0.15f;
    public float fadeOutDuration = 0.15f;

    [Header("Sorting")]
    public int sortingOrderOffset = 0;
}
