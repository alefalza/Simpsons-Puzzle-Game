using GameModes.BubbleMerge.Core;
using UnityEngine;

namespace GameModes.BubbleMerge.Gameplay
{
    public class CeilingTriggerDetector : MonoBehaviour
    {
        private bool gameOverTriggered = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (gameOverTriggered) return;
            if (!collision.TryGetComponent(out Bubble bubble)) return;
            if (collision.transform.position.y > transform.position.y) return;

            gameOverTriggered = true;
            Debug.Log("[Ceiling] Bubble touched ceiling → GAME OVER");

            BubbleGameManager.Instance.OnGameOver();
        }
    }
}
