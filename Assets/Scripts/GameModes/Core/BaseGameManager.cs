using Core;
using Core.Services.LevelProgressionService;
using UI;
using UnityEngine;

namespace GameModes.Core
{
    public abstract class BaseGameManager<T> : MonoBehaviour where T : BaseGameManager<T>
    {
        [Header("Level Configuration")]
        [Tooltip("If not assigned, will load automatically from LevelProgressionService")]
        [SerializeField] protected LevelDefinition levelData;
        
        [SerializeField] protected BaseHUDController hudController;
        
        public static T Instance { get; private set; }

        protected virtual string GameModeName => typeof(T).Name;
        
        protected int currentLevelNumber = 1;
        
        public bool IsPaused { get; protected set; }
        public bool IsInputBlocked { get; protected set; }

        protected virtual void Awake()
        {
            if (Instance == null)
                Instance = this as T;
            else
                Destroy(gameObject);

            LoadLevelFromProgression();
        }

        protected virtual void Start() { }

        protected virtual void Update()
        {
            HandleInput();
        }
        
        private void LoadLevelFromProgression()
        {
            // If levelData is manually assigned, don't override it (for testing purposes)
            if (levelData != null)
            {
                Debug.Log("[BubbleGameManager] Using manually assigned level definition");
                return;
            }

            currentLevelNumber = LevelProgressionService.GetNextPlayableLevel(GameModeName);
            
            var levelDef = LevelProgressionService.GetNextPlayableLevelDefinition(GameModeName);
            
            if (levelDef != null)
            {
                levelData = levelDef;
                Debug.Log($"[BubbleGameManager] Loaded level {currentLevelNumber}: {levelDef.name}");
            }
            else
            {
                Debug.LogWarning($"[BubbleGameManager] Could not load level definition for level {currentLevelNumber}");
            }
        }

        protected virtual void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }

            if (IsInputBlocked) return;
        }

        #region Pause Logic
        
        protected virtual void TogglePause()
        {
            if (hudController == null || !hudController.CanTogglePause()) return;

            if (!IsPaused)
                Pause();
            else
                Resume();
        }

        protected virtual void Pause()
        {
            IsPaused = true;
            IsInputBlocked = true;
            Time.timeScale = 0f;

            if (hudController != null)
            {
                hudController.ShowPausePopup();
            }

            OnPaused();
        }

        protected virtual void Resume()
        {
            IsPaused = false;
            IsInputBlocked = false;
            Time.timeScale = 1f;

            if (hudController != null)
            {
                hudController.HidePausePopup();
            }

            OnResumed();
        }

        public virtual void TogglePauseFromOverlay()
        {
            Resume();
        }

        protected virtual void OnPaused() { }

        protected virtual void OnResumed() { }
        
        #endregion

        protected void MarkLevelAsCompleted()
        {
            LevelProgressionService.CompleteLevel(GameModeName, currentLevelNumber);
            Debug.Log($"[BubbleGameManager] Level {currentLevelNumber} completed!");
        }
        
        protected virtual void OnDestroy()
        {
            if (Instance == this as T)
            {
                Instance = null;
            }
        }
        
        private ILevelProgressionService levelProgressionService;
        private ILevelProgressionService LevelProgressionService => levelProgressionService ??= ServiceLocator.Get<ILevelProgressionService>();
    }
}
