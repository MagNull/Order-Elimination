using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderElimination;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
using UnityEngine;
using XNode;

namespace RoguelikeMap.Points.Models
{
    [NodeWidth(300)]
    public abstract class PointModel : Node
    {
        [SerializeField]
        private string _name;
        
        [field: SerializeField]
        public Sprite Sprite { get; private set; }
        [field: SerializeField]
        public string TransferText { get; private set; }
        
        protected Panel _panel;
        public virtual PointType Type => PointType.None;
        public int Index { get; private set; }
        
        public event Action<bool> OnChangeActivity;

        public virtual void ShowPreview(Squad squad) => 
            Logging.LogException(
                new NotImplementedException("ShowPreviewPoint is not implemented in PointModel"));
        
        public virtual async Task Visit(Squad squad)
        {
            await squad.Visit(this);
        }

        public void SetIndex(int index) => Index = index;
        
        public void SetPanel(PanelManager panelManager) => _panel = panelManager.GetPanelByPointInfo(Type);

        public IEnumerable<PointModel> GetNextPoints()
        {
            return !HasPort("exits") ? new List<PointModel>() 
                : GetPort("exits").GetConnections().Select(connection => connection.node as PointModel);
        }

        public void SetActive(bool isActive) => OnChangeActivity?.Invoke(isActive);

        private void OnValidate() => name = _name == "" ? GetType().Name : _name;
    }
}