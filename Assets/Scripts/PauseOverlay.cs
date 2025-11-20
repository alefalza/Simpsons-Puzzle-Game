using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Core;
using Core.Managers;

public class PauseOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;

    private SceneController sceneController;

    public bool IsFading { get; private set; } = false;
    
    private void Awake()
    {
        sceneController = ServiceLocator.Get<SceneController>();

        resumeButton.onClick.AddListener(OnResumePressed);
        menuButton.onClick.AddListener(OnMenuPressed);
    }

    public void Show()
    {
        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        Time.timeScale = 1f;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        IsFading = true;
        canvasGroup.alpha = 0;
        
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * 4f;
            yield return null;
        }
        
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        Time.timeScale = 0f;
        IsFading = false;
    }

    private IEnumerator FadeOut()
    {
        IsFading = true;
        
        canvasGroup.alpha = 1;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * 4f;
            yield return null;
        }

        IsFading = false;
    }

    private void OnResumePressed()
    {
        Hide();
    }

    private void OnMenuPressed()
    {
        sceneController.LoadScene("MainMenuScene");
    }
}
