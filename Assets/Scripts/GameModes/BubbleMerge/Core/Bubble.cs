using UnityEngine;

namespace GameModes.BubbleMerge.Core
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private int tier = 0;
        [SerializeField] private float mergeCooldown = 0.15f;

        public int Tier => tier;

        public bool HasMerged { get; private set; }
        public bool IsMergeBlocked { get; private set; }

        public void MarkAsMerged()
        {
            HasMerged = true;
        }

        public void BlockMergeTemporarily()
        {
            IsMergeBlocked = true;
            Invoke(nameof(UnblockMerge), mergeCooldown);
        }

        private void UnblockMerge()
        {
            IsMergeBlocked = false;
        }
    }
}
