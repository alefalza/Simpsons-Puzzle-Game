using GameModes.BubbleMerge.Core;
using GameModes.BubbleMerge.UI;
using UnityEngine;

namespace GameModes.BubbleMerge.Gameplay
{
    public class BubbleGameManager : MonoBehaviour
    {
        [SerializeField] private BubbleSpawner spawner;
        [SerializeField] private BubbleHUDController HUDController;

        private int score = 0;
        private bool isInputBlocked = false;

        public static BubbleGameManager Instance;

        public int MaxTier => spawner.MaxTier;

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
            if (isInputBlocked) return;

            if (Input.GetMouseButtonDown(0))
            {
                spawner.DropBubble();
            }
        }

        private void InitHUDController()
        {
            HUDController.UpdateScore(0);
            UpdateHUD(spawner.CurrentTier, spawner.NextTier);
        }

        public void UpdateHUD(int current, int next)
        {
            HUDController.UpdateCurrentBubbleIcon(current);
            HUDController.UpdateNextBubbleIcon(next);
        }

        public Bubble SpawnMergedBubble(int tier, Vector3 position)
        {
            AddScore(tier * 10);
            return spawner.SpawnBubble(tier, position);
        }

        private void AddScore(int amount)
        {
            score += amount;
            HUDController.UpdateScore(score);
        }

        public void OnGameOver()
        {
            isInputBlocked = true;
            HUDController.ShowGameOver(score);
        }
    }
}
