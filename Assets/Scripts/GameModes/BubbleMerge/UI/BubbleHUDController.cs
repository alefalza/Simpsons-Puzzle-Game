using UnityEngine;
using UnityEngine.UI;

namespace GameModes.BubbleMerge.UI
{
    public class BubbleHUDController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private BubbleScoreUI scoreUI;
        [SerializeField] private Image currentBubbleIcon;
        [SerializeField] private Image nextBubbleIcon;

        [Header("Bubble Icons")]
        [SerializeField] private Sprite[] bubbleTierIcons;

        public void UpdateScore(int newScore)
        {
            scoreUI.SetScore(newScore);
        }

        public void UpdateCurrentBubbleIcon(int tier)
        {
            if (tier < 0 || tier >= bubbleTierIcons.Length) return;

            currentBubbleIcon.sprite = bubbleTierIcons[tier];
        }

        public void UpdateNextBubbleIcon(int tier)
        {
            if (tier < 0 || tier >= bubbleTierIcons.Length) return;

            nextBubbleIcon.sprite = bubbleTierIcons[tier];
        }
    }
}
