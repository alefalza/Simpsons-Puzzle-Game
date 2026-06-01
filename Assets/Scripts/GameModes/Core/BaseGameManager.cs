using Core;
using Core.Services.LevelProgressionService;
using Core.Services.SceneService;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameModes.Core
{
    public abstract class BaseGameManager<T> : MonoBehaviour, IGameSession where T : BaseGameManager<T>
    {
        [Header("Level Configuration")]
        [Tooltip("If not assigned, will load automatically from LevelProgressionService")]
        [SerializeField] protected LevelDefinition levelData;
        [SerializeField] protected BaseHUDController hudController;
        
        public static T Instance { get; private set; }

        protected virtual string GameModeName => typeof(T).Name;
        
        protected int currentLevelNumber = 1;
        protected bool hasWon = false;
        protected bool hasLost = false;
        
        public bool IsPaused { get; private set; }
        public bool IsInputBlocked { get; protected set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                GameSession.Current = this;
            }
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
                Debug.Log($"[{GameModeName}] Using manually assigned level definition");
                return;
            }
            
            var levelDef = LevelProgressionService.GetNextPlayableLevelDefinition(GameModeName);
            
            if (levelDef != null)
            {
                currentLevelNumber = levelDef.levelNumber;
                levelData = levelDef;
                Debug.Log($"[{GameModeName}] Loaded level {currentLevelNumber}: {levelDef.name}");
            }
            else
            {
                Debug.LogWarning($"[{GameModeName}] Could not load level definition for level {currentLevelNumber}");
            }
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }

            if (IsInputBlocked) return;
        }

        #region Pause Logic
        public void TogglePause()
        {
            if (hudController == null || !hudController.CanTogglePause()) return;

            if (!IsPaused)
                Pause();
            else
                Resume();
        }

        private void Pause()
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

        private void Resume()
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

        protected virtual void OnPaused() { }

        protected virtual void OnResumed() { }
        #endregion

        public virtual void Retry()
        {
            Time.timeScale = 1f;
            SceneService.LoadScene(SceneManager.GetActiveScene().name);
        }

        protected void MarkLevelAsCompleted()
        {
            LevelProgressionService.CompleteLevel(GameModeName, currentLevelNumber);
            Debug.Log($"[{GameModeName}] Level {currentLevelNumber} is completed!");
        }

        protected virtual void OnGameWon(int finalScore = 0)
        {
            hasWon = true;
            
            if (hudController != null)
            {
                hudController.ShowWinPopup(finalScore);
            }
        }

        protected virtual void OnGameLost(int finalScore = 0)
        {
            hasLost = true;
            
            if (hudController != null)
            {
                hudController.ShowGameOverPopup(finalScore);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this as T)
                Instance = null;

            if (ReferenceEquals(GameSession.Current, this))
                GameSession.Current = null;
        }
        
        private ILevelProgressionService levelProgressionService;
        private ILevelProgressionService LevelProgressionService => levelProgressionService ??= ServiceLocator.Get<ILevelProgressionService>();

        private SceneService sceneService;
        private SceneService SceneService => sceneService ??= ServiceLocator.Get<SceneService>();
    }
}
