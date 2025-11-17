using GameModes.BubbleMerge.Gameplay;
using UnityEngine;

namespace GameModes.BubbleMerge.Core
{
    public class BubbleMergeDetector : MonoBehaviour
    {
        private Bubble bubble;

        private void Awake()
        {
            bubble = GetComponent<Bubble>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Bubble other = collision.collider.GetComponent<Bubble>();

            if (other == null) return;
            if (other.tier != bubble.tier) return;
            if (bubble.hasMerged || other.hasMerged) return; // TODO: remove eventually

            BubbleGameManager.Instance.TryMerge(bubble, other);
        }
    }
}
