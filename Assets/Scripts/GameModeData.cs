using UnityEngine;

[CreateAssetMenu(menuName = "GameModes/GameModeData")]
public class GameModeData : ScriptableObject
{
    public string modeName;
    public string sceneName;
    public Sprite icon;
}
