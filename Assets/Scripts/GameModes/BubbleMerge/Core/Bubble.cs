using UnityEngine;

namespace GameModes.BubbleMerge.Core
{
    public class Bubble : MonoBehaviour
    {
        public int tier;                      // Tier (0 = smallest)
        public bool hasMerged = false;        // Prevents double merges

        [HideInInspector] public Rigidbody2D rb;
        [HideInInspector] public CircleCollider2D col;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CircleCollider2D>();
        }
    }
}
