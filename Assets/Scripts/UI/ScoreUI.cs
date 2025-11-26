using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        public void SetScore(int score)
        {
            if (scoreText != null)
                scoreText.text = "Score: " + score;
        }
    }
}
