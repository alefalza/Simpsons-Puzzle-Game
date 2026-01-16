using UI;
using UnityEngine;

namespace GameModes.Core
{
    public abstract class BaseGameManager<T> : MonoBehaviour where T : BaseGameManager<T>
    {
        [SerializeField] protected BaseHUDController hudController;
        
        public static T Instance { get; private set; }

        public bool IsPaused { get; protected set; }
        public bool IsInputBlocked { get; protected set; }

        protected virtual void Awake()
        {
            if (Instance == null)
                Instance = this as T;
            else
                Destroy(gameObject);
        }

        protected virtual void Start() { }

        protected virtual void Update()
        {
            HandleInput();
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

        protected virtual void OnDestroy()
        {
            if (Instance == this as T)
            {
                Instance = null;
            }
        }
    }
}
