using GameModes.DrinkSort.Gameplay;
using TMPro;
using UI;
using UnityEngine;

namespace GameModes.DrinkSort.UI
{
    public class DrinkSortHUDController : BaseHUDController
    {
        [Header("Timer UI")]
        [SerializeField] private TMP_Text timerText;
        
        [Header("Items Remaining UI")]
        [SerializeField] private TMP_Text itemsRemainingText;
        
        protected override void OnResumeClicked()
        {
            DrinkSortGameManager.Instance.TogglePauseFromOverlay();
        }
        
        public void UpdateTimer(float timeRemaining)
        {
            if (timerText == null) return;
            
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            
            timerText.text = $"{minutes:00}:{seconds:00}";
            
            // Cambiar color cuando el tiempo es bajo (opcional)
            if (timeRemaining <= 30f)
            {
                timerText.color = Color.red;
            }
            else if (timeRemaining <= 60f)
            {
                timerText.color = Color.yellow;
            }
            else
            {
                timerText.color = Color.white;
            }
        }
        
        public void UpdateItemsRemaining(int itemsRemaining)
        {
            if (itemsRemainingText == null) return;
            
            itemsRemainingText.text = $"Items: {itemsRemaining}";
        }
    }
}
