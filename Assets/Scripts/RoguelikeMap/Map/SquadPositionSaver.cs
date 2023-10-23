using System;
using OrderElimination;
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
        private ScenesMediator _mediator;

        private const string PassPointKey = "is point passed";

        public event Action<int> OnSaveBeforeMove;
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
            _mediator = mediator;
            Subscribe();
        }

        public void SavePosition(int pointIndex)
        {
            _mediator.Register("point index", pointIndex);
            
        }

        public int GetPointIndex()
        {
            return _mediator.Contains<int>("point index")
                ? _mediator.Get<int>("point index") : -1;
        }

        public bool IsPassedPoint()
        {
            return _mediator.Contains<bool>(PassPointKey) && _mediator.Get<bool>(PassPointKey);
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

        private void LeavePoint(int pointIndex)
        {
            SavePosition(pointIndex);
            SwitchPoint();
            OnSaveBeforeMove?.Invoke(pointIndex);
        }
        
        public void SwitchPoint()
        {
            Debug.Log("Pass point false");
            if(_mediator.Contains<bool>(PassPointKey))
                _mediator.Unregister(PassPointKey);
            _mediator.Register(PassPointKey, false);
        }
        
        public void PassPoint()
        {
            Debug.Log("Pass point true");
            if(_mediator.Contains<bool>(PassPointKey))
                _mediator.Unregister(PassPointKey);
            _mediator.Register(PassPointKey, true);
            OnPassPoint?.Invoke();
        }
    }
}