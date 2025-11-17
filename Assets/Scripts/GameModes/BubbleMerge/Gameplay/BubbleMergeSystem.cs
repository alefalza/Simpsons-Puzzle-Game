using GameModes.BubbleMerge.Core;
using UnityEngine;

namespace GameModes.BubbleMerge.Gameplay
{
    public class BubbleMergeSystem
    {
        public Bubble Merge(Bubble a, Bubble b)
        {
            a.hasMerged = true;
            b.hasMerged = true;

            Vector3 spawnPos = (a.transform.position + b.transform.position) * 0.5f;

            Object.Destroy(a.gameObject);
            Object.Destroy(b.gameObject);

            return BubbleGameManager.Instance.SpawnMergedBubble(a.tier + 1, spawnPos);
        }
    }
}
