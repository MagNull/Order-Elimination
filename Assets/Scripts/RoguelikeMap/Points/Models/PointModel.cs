using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
using UnityEngine;
using XNode;

namespace RoguelikeMap.Points.Models
{
    public abstract class PointModel : Node
    {
        [field: SerializeField]
        public Sprite Sprite { get; private set; }

        protected Panel _panel;
        public virtual PointType Type => PointType.None;
        
        public virtual void Visit(Squad squad)
        {
            squad.Visit(this);
        }

        public void SetPanel(PanelManager panelManager) => _panel = panelManager.GetPanelByPointInfo(Type);

        public void ShowPanel()
        {
            if (_panel is null)
                return;
            _panel.Open();
        }

        public virtual IEnumerable<PointModel> GetNextPoints()
        {
            return GetPort("exits").GetConnections().Select(connection => connection.node as PointModel);
        }
    }
}