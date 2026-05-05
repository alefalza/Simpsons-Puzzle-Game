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

        [Header("Spawn Settings")]
        [Min(0f)]
        [SerializeField] private float spawnCooldown = 0.15f;
        
        private Camera mainCamera;
        private bool isDragging = false;
        private int[] levelSpawnWeights;
        private float nextAllowedSpawnTime;
        private Vector3 aimLineEndPosition;
        
        public int CurrentTier { get; private set; }
        public int NextTier { get; private set; }
        public int MaxTier => bubblePrefabs.Length - 1;
        public Bubble[] BubblePrefabs => bubblePrefabs;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public void Init(BubbleMergeLevelDefinition levelDefinition)
        {
            levelSpawnWeights = levelDefinition != null
                ? levelDefinition.GetSpawnWeights(bubblePrefabs.Length)
                : null;

            CurrentTier = GetRandomWeightedTier();
            NextTier = GetRandomWeightedTier();
            nextAllowedSpawnTime = 0f;
        }

        private void Update()
        {
            HandleInput();
            FollowMouseWhileDragging();
            UpdateAimLine();
        }

        private void HandleInput()
        {
            if (BubbleGameManager.Instance.IsInputBlocked) return;

            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                spawnPoint.position = GetClampedMousePos();
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

        private Vector3 GetClampedMousePos()
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            float clampedX = Mathf.Clamp(worldPos.x, -horizontalLimit, horizontalLimit);

            return new Vector3(clampedX, spawnPoint.position.y, spawnPoint.position.z);
        }
        
        private void FollowMouseWhileDragging()
        {
            if (!isDragging || BubbleGameManager.Instance.IsInputBlocked) return;
            spawnPoint.position = Vector3.Lerp(spawnPoint.position, GetClampedMousePos(), followSpeed * Time.deltaTime);
        }
        
        private void UpdateAimLine()
        {
            Vector3 start = spawnPoint.position;
            aimLineEndPosition = start + Vector3.down * maxRayDistance;
            
            RaycastHit2D hit = Physics2D.Raycast(start, Vector2.down, maxRayDistance, groundMask);

            if (hit.collider != null)
            {
                aimLineEndPosition = hit.point;
            }

            aimLine.positionCount = 2;
            aimLine.SetPosition(0, start);
            aimLine.SetPosition(1, aimLineEndPosition);
        }
        
        private void DropBubble()
        {
            if (Time.time < nextAllowedSpawnTime) return;
            nextAllowedSpawnTime = Time.time + spawnCooldown;

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

            for (int i = 0; i < bubblePrefabs.Length; i++)
            {
                totalWeight += GetWeightForTier(i);
            }

            if (totalWeight <= 0)
                return 0;

            int randomValue = Random.Range(0, totalWeight);
            int cumulative = 0;

            for (int i = 0; i < bubblePrefabs.Length; i++)
            {
                cumulative += GetWeightForTier(i);
                if (randomValue < cumulative)
                    return i;
            }

            return 0;
        }

        private int GetWeightForTier(int tierIndex)
        {
            if (levelSpawnWeights == null ||
                tierIndex < 0 ||
                tierIndex >= levelSpawnWeights.Length)
            {
                // Si no hay configuración válida, no se genera este tier
                return 0;
            }

            return Mathf.Max(0, levelSpawnWeights[tierIndex]);
        }
    }
}
