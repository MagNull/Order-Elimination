using System;
using System.Collections.Generic;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
using UnityEngine;

namespace RoguelikeMap.Points
{
    [Serializable]
    public class PointModel
    {
        [SerializeField]
        private List<int> _nextPointsIndex;
        [SerializeField]
        private Vector3 _position;
        [field: SerializeField]
        public bool IsLastPoint { get; private set; }

        protected Panel _panel;
        
        public int Index;
        
        public Vector3 Position => _position;
        public IReadOnlyList<int> NextPoints => _nextPointsIndex;
        public int PointIndex => Index;
        public virtual PointType Type => PointType.None;
        public event Action<PointModel> OnSelected;

        public virtual void Visit(Squad squad)
        {
            squad.Visit(this);
            PlayerPrefs.SetInt(Map.Map.SquadPositionKey, Index);
        }

        public void SetPanel(PanelManager panelManager) => _panel = panelManager.GetPanelByPointInfo(Type);
    }
}