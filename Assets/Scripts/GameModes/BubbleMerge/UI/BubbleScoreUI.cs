using TMPro;
using UnityEngine;

namespace GameModes.BubbleMerge.UI
{
    public class BubbleScoreUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        public void SetScore(int score)
        {
            if (scoreText != null)
                scoreText.text = "Score:" + score;
        }
    }
}
