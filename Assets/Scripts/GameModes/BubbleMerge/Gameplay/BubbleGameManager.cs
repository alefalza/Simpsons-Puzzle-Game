using GameModes.BubbleMerge.Core;
using GameModes.BubbleMerge.UI;
using UnityEngine;

namespace GameModes.BubbleMerge.Gameplay
{
    public class BubbleGameManager : MonoBehaviour
    {
        [SerializeField] private BubbleSpawner spawner;
        [SerializeField] private BubbleHUDController hudController;

        private int score = 0;
        private bool isInputBlocked = false;

        public static BubbleGameManager Instance;

        public int MaxTier => spawner.MaxTier;
        public bool IsPaused { get; private set; } = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            score = 0;
            spawner.Init();
            InitHUDController();
        }

        private void Update()
        {
            GetInput();
        }

        private void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                OnPause();
            
            if (isInputBlocked) return;

            if (Input.GetMouseButtonDown(0))
                spawner.DropBubble();
        }

        private void InitHUDController()
        {
            hudController.UpdateScore(0);
            UpdateHUD(spawner.CurrentTier, spawner.NextTier);
        }

        public void UpdateHUD(int current, int next)
        {
            hudController.UpdateCurrentBubbleIcon(current);
            hudController.UpdateNextBubbleIcon(next);
        }

        public Bubble SpawnMergedBubble(int tier, Vector3 position)
        {
            AddScore(tier * 10);
            return spawner.SpawnBubble(tier, position);
        }

        private void AddScore(int amount)
        {
            score += amount;
            hudController.UpdateScore(score);
        }

        private void OnPause()
        {
            if (hudController.TryShowPauseOverlay(IsPaused))
            {
                IsPaused = !IsPaused;
                isInputBlocked = IsPaused;
            }
        }

        public void OnGameOver()
        {
            isInputBlocked = true;
            hudController.ShowGameOverOverlay(score);
        }
    }
}
