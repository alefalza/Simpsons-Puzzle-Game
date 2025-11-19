using GameModes.BubbleMerge.Gameplay;
using UnityEngine;

namespace GameModes.BubbleMerge.Core
{
    public class BubbleMergeDetector : MonoBehaviour
    {
        private Bubble bubble;
        private BubbleMergeSystem mergeSystem;

        private void Awake()
        {
            bubble = GetComponent<Bubble>();
            mergeSystem = new BubbleMergeSystem();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.collider.TryGetComponent<Bubble>(out var other)) return;
            if (other.Tier != bubble.Tier) return;

            if (bubble.IsMergeBlocked || other.IsMergeBlocked) return;
            if (bubble.HasMerged || other.HasMerged) return;

            mergeSystem.Merge(bubble, other);
        }
    }
}
