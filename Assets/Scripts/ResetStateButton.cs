using Core;
using Core.Services.LevelProgressionService;
using UnityEngine;

public class ResetStateButton : MonoBehaviour
{
    public void ResetState()
    {
        LevelProgressionService.ResetProgressionAndDeleteSaved();
    }
    
    private ILevelProgressionService levelProgressionService;
    private ILevelProgressionService LevelProgressionService => levelProgressionService ??= ServiceLocator.Get<ILevelProgressionService>();
}
