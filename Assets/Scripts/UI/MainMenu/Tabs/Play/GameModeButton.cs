using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Services;
using Core.Services.SceneService;

public class GameModeButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;

    private Button button;
    private GameModeData data;
    private SceneService sceneController;

    private void Awake()
    {
        button =  GetComponent<Button>();
        sceneController = ServiceLocator.Get<SceneService>();
    }

    public void Initialize(GameModeData modeData)
    {
        data = modeData;

        if (label != null)
            label.text = data.modeName;

        if (icon != null && data.icon != null)
            icon.sprite = data.icon;

        button.onClick.AddListener(() =>
        {
            sceneController.LoadScene(data.sceneName);
        });
    }
}
