using UnityEngine;

namespace GameModes.BubbleMerge.Core
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private int tier = 0;
        [SerializeField] private int spawnWeight = 100;
        [SerializeField] private float mergeCooldown = 0.15f;

        private Rigidbody2D rb;

        public int Tier => tier;
        public int SpawnWeight => spawnWeight;

        public bool HasMerged { get; private set; }
        public bool IsMergeBlocked { get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

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
