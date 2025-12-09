using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Priority
{
    Urgent = 4,
    High = 3,
    Medium = 2,
    Low = 1
}

namespace Core.Services.PopupService
{
    [Serializable]
    public class PopupService : IPopupService
    {
        private List<PopupData> _popupsToShow;
        private BasePopup _openedPopup;
        private BasePopup _hidedPopup;
        private readonly IPopupFactory _popupsFactory;
        private readonly Transform _prefabsRoot;
        
        private readonly Dictionary<string, BasePopup> activePopups = new();

        public PopupService(IPopupFactory popupsFactory, Transform prefabsRoot)
        {
            _popupsFactory = popupsFactory;
            _prefabsRoot = prefabsRoot;
            _popupsToShow = new List<PopupData>();
        }

        public void Initialize()
        {
            Debug.Log("[PopupService] Initializing...");
            
        }

        public void PushPopup(PopupData data)
        {
            if (data == null)
            {
                Debug.LogError($"[{nameof(PopupService)}] - Unable to push message. Null APopupData");
                return;
            }

            _popupsToShow ??= new List<PopupData>();
            _popupsToShow.Add(data);
            ShowPopup();
        }
        
        private void ShowPopup()
        {
            if (_openedPopup == null && _hidedPopup != null)
            {
                _hidedPopup.Open();
                _openedPopup = _hidedPopup;
                _hidedPopup = null;
                return;
            }

            var popupData = GetPopupToShow();
            if (popupData == null || (_openedPopup != null && popupData.Priority != Priority.Urgent)) return;

            if (popupData.Priority == Priority.Urgent && _openedPopup != null)
            {
                _openedPopup.Hide();
                _hidedPopup = _openedPopup;
                _openedPopup = null;
            }

            _popupsToShow.Remove(popupData);
            _openedPopup = _popupsFactory.InstantiatePopup(popupData, _prefabsRoot);
            _openedPopup.OnClosed += PopupOnClosedHandler;
            _openedPopup.Open();
        }
        
        
        private void PopupOnClosedHandler()
        {
            _openedPopup.OnClosed -= PopupOnClosedHandler;
            _openedPopup = null;
            ShowPopup();
        }
        
        private PopupData GetPopupToShow()
        {
            if (GetQueueSize() == 0) return null;
        
            var prioritizedList = _popupsToShow.OrderByDescending(t => t.Priority);
            return prioritizedList.FirstOrDefault();
        }

        public int GetQueueSize()
        {
            return _popupsToShow?.Count ?? 0;
        }

        public BasePopup GetOpenedPopup()
        {
            return _openedPopup;
        }

        public void Shutdown()
        {
            Debug.Log("[PopupService] Shutting down...");
        }
    }
}
