using Core;
using Core.Services.AudioService;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound) AudioService.PlaySFX(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound) AudioService.PlaySFX(clickSound);
    }

    private IAudioService audioService;
    private IAudioService AudioService => audioService ??= ServiceLocator.Get<IAudioService>();
}
