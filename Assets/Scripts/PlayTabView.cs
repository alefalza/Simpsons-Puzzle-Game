using UnityEngine;

public class PlayTabView : MonoBehaviour
{
    [Header("Grid Parent")]
    [SerializeField] private Transform gridContainer;

    [Header("Button Prefab")]
    [SerializeField] private GameModeButton buttonPrefab;

    [Header("Modes List")]
    [SerializeField] private GameModeData[] gameModes;

    private void Start()
    {
        foreach (var mode in gameModes)
        {
            var button = Instantiate(buttonPrefab, gridContainer);
            button.Initialize(mode);
        }
    }
}
