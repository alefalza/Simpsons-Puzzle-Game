using GameModes.BubbleMerge.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModes.BubbleMerge.Gameplay
{
    public class BubbleSpawner : MonoBehaviour
    {
        [SerializeField] private Bubble[] bubblePrefabs;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform bubbleRoot;

        [Header("Aim Line Settings")]
        [SerializeField] private float horizontalLimit = 2.5f;
        [SerializeField] private float followSpeed = 15f;
        [SerializeField] private LineRenderer aimLine;
        [SerializeField] private float maxRayDistance = 20f;
        [SerializeField] private LayerMask groundMask;
        
        private Camera mainCamera;
        private bool isDragging = false;
        
        public int CurrentTier { get; private set; }
        public int NextTier { get; private set; }
        public int MaxTier => bubblePrefabs.Length - 1;
        public Bubble[] BubblePrefabs => bubblePrefabs;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public void Init()
        {
            CurrentTier = GetRandomWeightedTier();
            NextTier = GetRandomWeightedTier();
        }

        private void Update()
        {
            HandleInput();
            FollowMouseWhileDragging();
            UpdateAimLine();
        }

        private void HandleInput()
        {
            if (BubbleGameManager.Instance.IsPaused) return;

            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isDragging)
                {
                    isDragging = false;
                    DropBubble();
                }
            }
        }

        private void FollowMouseWhileDragging()
        {
            if (!isDragging) return;

            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            float clampedX = Mathf.Clamp(worldPos.x, -horizontalLimit, horizontalLimit);

            Vector3 target = new Vector3(clampedX, spawnPoint.position.y, spawnPoint.position.z);

            spawnPoint.position = Vector3.Lerp(
                spawnPoint.position,
                target,
                followSpeed * Time.deltaTime
            );
        }
        
        private void UpdateAimLine()
        {
            Vector3 start = spawnPoint.position;
            Vector3 end = start + Vector3.down * maxRayDistance;
            
            RaycastHit2D hit = Physics2D.Raycast(start, Vector2.down, maxRayDistance, groundMask);

            if (hit.collider != null)
            {
                end = hit.point;
            }

            aimLine.positionCount = 2;
            aimLine.SetPosition(0, start);
            aimLine.SetPosition(1, end);
        }
        
        private void DropBubble()
        {
            SpawnBubble(CurrentTier, spawnPoint.position);

            CurrentTier = NextTier;
            NextTier = GetRandomWeightedTier();

            BubbleGameManager.Instance.UpdateHUD(CurrentTier, NextTier);
        }

        public Bubble SpawnBubble(int tier, Vector3 position)
        {
            return Instantiate(bubblePrefabs[tier], position, Quaternion.identity, bubbleRoot);
        }

        private int GetRandomWeightedTier()
        {
            int totalWeight = 0;

            foreach (var bubble in bubblePrefabs)
                totalWeight += bubble.SpawnWeight;

            if (totalWeight <= 0)
                return 0;

            int randomValue = Random.Range(0, totalWeight);
            int cumulative = 0;

            for (int i = 0; i < bubblePrefabs.Length; i++)
            {
                cumulative += bubblePrefabs[i].SpawnWeight;
                if (randomValue < cumulative)
                    return i;
            }

            return 0;
        }
    }
}
