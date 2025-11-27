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

        public static BubbleGameManager Instance;

        public int MaxTier => spawner.MaxTier;
        public bool IsPaused { get; private set; } = false;
        public bool IsInputBlocked { get; private set; } = false;

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
                TogglePause();
            
            if (IsInputBlocked) return;
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

        public Bubble GetBubblePrefabByTier(int tier)
        {
            return spawner.BubblePrefabs[tier];
        }
        
        private void AddScore(int amount)
        {
            score += amount;
            hudController.UpdateScore(score);
        }

        #region Pause Logic
        private void TogglePause()
        {
            if (!hudController.CanTogglePause()) return;

            if (!IsPaused)
                Pause();
            else
                Resume();
        }

        private void Pause()
        {
            IsPaused = true;
            IsInputBlocked = true;
            spawner.enabled = false;

            hudController.ShowPauseOverlay();
        }

        private void Resume()
        {
            IsPaused = false;
            IsInputBlocked = false;
            spawner.enabled = true;

            hudController.HidePauseOverlay();
        }

        public void TogglePauseFromOverlay()
        {
            Resume();
        }
        #endregion

        public void OnGameOver()
        {
            IsInputBlocked = true;
            hudController.ShowGameOverOverlay(score);
        }
    }
}
