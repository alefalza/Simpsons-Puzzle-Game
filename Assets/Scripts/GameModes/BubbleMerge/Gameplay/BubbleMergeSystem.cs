using GameModes.BubbleMerge.Core;
using UnityEngine;

namespace GameModes.BubbleMerge.Gameplay
{
    public class BubbleMergeSystem
    {
        public Bubble Merge(Bubble a, Bubble b)
        {
            a.MarkAsMerged();
            b.MarkAsMerged();

            int maxTier = BubbleGameManager.Instance.MaxTier;

            if (a.Tier >= maxTier && b.Tier >= maxTier)
                return null;

            a.BlockMergeTemporarily();
            b.BlockMergeTemporarily();

            Object.Destroy(a.gameObject);
            Object.Destroy(b.gameObject);

            Vector3 spawnPos = (a.transform.position + b.transform.position) * 0.5f;
            Bubble mergedBubble = BubbleGameManager.Instance.SpawnMergedBubble(a.Tier + 1, spawnPos);
            mergedBubble.BlockMergeTemporarily();

            return mergedBubble;
        }
    }
}
