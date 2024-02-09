using System;
using OrderElimination;
using OrderElimination.SavesManagement;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.UI;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.Map
{
    [Serializable]
    public class SquadPositionSaver
    {
        private TransferPanel _transferPanel;
        private BattlePanel _battlePanel;
        private ShopPanel _shopPanel;
        private EventPanel _eventPanel;
        private SafeZonePanel _safeZonePanel;
        private IPlayerProgressManager _progressManager;

        public event Action<Guid> OnSaveBeforeMove;
        public event Action OnPassPoint; 

        [Inject]
        public SquadPositionSaver(PanelManager panelManager, TransferPanel transferPanel, 
            ScenesMediator mediator)
        {
            _transferPanel = transferPanel;
            _battlePanel = panelManager.GetPanelByPointInfo(PointType.Battle) as BattlePanel;
            _shopPanel = panelManager.GetPanelByPointInfo(PointType.Shop) as ShopPanel;
            _eventPanel = panelManager.GetPanelByPointInfo(PointType.Event) as EventPanel;
            _safeZonePanel = panelManager.GetPanelByPointInfo(PointType.SafeZone) as SafeZonePanel;
            _progressManager = mediator.Get<IPlayerProgressManager>(MediatorRegistration.ProgressManager);
            Subscribe();
        }

        public void SavePosition(Guid pointId)
        {
            _progressManager.GetPlayerProgress().CurrentRunProgress.CurrentPointId = pointId;
        }

        public Guid GetPointIndex()
        {
            return _progressManager.GetPlayerProgress().CurrentRunProgress.CurrentPointId;
        }

        public bool IsPassedPoint()
        {
            var currentPointId = _progressManager.GetPlayerProgress().CurrentRunProgress.CurrentPointId;
            return _progressManager.GetPlayerProgress().CurrentRunProgress.PassedPoints[currentPointId];
        }

        private void Subscribe()
        {
            _transferPanel.OnAccept += LeavePoint;
            _battlePanel.OnAccepted += LeavePoint;
            
            _shopPanel.OnClose += PassPoint;
            _eventPanel.OnClose += PassPoint;
            _safeZonePanel.OnClose += PassPoint;
        }
        
        private void Unsubscribe()
        {
            _transferPanel.OnAccept -= LeavePoint;
            _battlePanel.OnAccepted -= LeavePoint;
            
            _shopPanel.OnClose -= PassPoint;
            _eventPanel.OnClose -= PassPoint;
            _safeZonePanel.OnClose -= PassPoint;
        }

        private void LeavePoint(Guid pointId)
        {
            SavePosition(pointId);
            OnSaveBeforeMove?.Invoke(pointId);
        }
        
        public void PassPoint()
        {
            var currentPointId = _progressManager.GetPlayerProgress().CurrentRunProgress.CurrentPointId;
            _progressManager.GetPlayerProgress().CurrentRunProgress.PassedPoints[currentPointId] = true;
            OnPassPoint?.Invoke();
        }
    }
}