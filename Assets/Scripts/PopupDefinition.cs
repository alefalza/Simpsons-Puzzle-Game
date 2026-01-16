using Core.Services.PopupService;
using UnityEngine;

namespace Core.Services.PopupService
{
    [CreateAssetMenu(fileName = "PopupDefinition", menuName = "Popups/Popup Definition")]
    public class PopupDefinition : ScriptableObject
{
    [Header("Identification")]
    public string id;

    [Header("Prefab")]
    public BasePopup prefab;

    [Header("Behavior")]
    public Priority defaultPriority = Priority.Low;
    public bool destroyOnClose = true;

    [Header("Animation")]
    public float fadeInDuration = 0.15f;
    public float fadeOutDuration = 0.15f;
    }
}
