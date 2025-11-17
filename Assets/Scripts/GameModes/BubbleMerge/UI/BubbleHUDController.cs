using UnityEngine;
using UnityEngine.UI;

namespace GameModes.BubbleMerge.UI
{
    public class BubbleHUDController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BubbleScoreUI scoreUI;
        [SerializeField] private Image currentBubbleIcon;
        [SerializeField] private Image nextBubbleIcon;

        [Header("Bubble Icons")]
        [SerializeField] private Sprite[] bubbleTierIcons; // One icon per tier

        private int currentScore = 0;

        private void Start()
        {
            //UpdateCurrentBubble(BubbleGameManager.Instance.BubbleSpawner.GetCurrentTier());
            //UpdateNextBubble(BubbleGameManager.Instance.BubbleSpawner.GetNextTier());
            UpdateScore(0);
        }

        public void UpdateScore(int newScore)
        {
            currentScore = newScore;
            scoreUI.SetScore(newScore);
        }

        public void UpdateCurrentBubble(int tier)
        {
            if (tier < 0 || tier >= bubbleTierIcons.Length) return;
            currentBubbleIcon.sprite = bubbleTierIcons[tier];
        }

        public void UpdateNextBubble(int tier)
        {
            if (tier < 0 || tier >= bubbleTierIcons.Length) return;
            nextBubbleIcon.sprite = bubbleTierIcons[tier];
        }
    }
}
