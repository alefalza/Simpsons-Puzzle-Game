using GameModes.BubbleMerge.Core;
using GameModes.BubbleMerge.Gameplay;
using UI;
using UnityEngine;

namespace GameModes.BubbleMerge.UI
{
    public class BubbleHUDController : BaseHUDController
    {
        [Header("UI")]
        [SerializeField] private BubbleIconUI currentBubbleIcon;
        [SerializeField] private BubbleIconUI nextBubbleIcon;

        [Header("Bubble Icons")]
        [SerializeField] private Sprite[] bubbleTierIcons;

        public void UpdateCurrentBubbleIcon(int tier)
        {
            if (tier < 0 || tier >= bubbleTierIcons.Length) return;

            Bubble bubblePrefab = BubbleGameManager.Instance.GetBubblePrefabByTier(tier);
            currentBubbleIcon.SetBubbleIcon(bubbleTierIcons[tier], bubblePrefab);
        }

        public void UpdateNextBubbleIcon(int tier)
        {
            if (tier < 0 || tier >= bubbleTierIcons.Length) return;

            nextBubbleIcon.SetBubbleIcon(bubbleTierIcons[tier]);
        }

        public override void OnResumeClicked()
        {
            BubbleGameManager.Instance.TogglePauseFromOverlay();
        }
    }
}
