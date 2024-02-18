using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderElimination.GameContent;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using XNode;

namespace RoguelikeMap.Points.Models
{
    [NodeWidth(300)]
    public abstract class PointModel : Node, IGuidAsset
    {
        [PropertyOrder(-1)]
        [OdinSerialize, DisplayAsString]
        public Guid AssetId { get; private set; }

        [SerializeField]
        protected string _name;

        [field: SerializeField]
        public Sprite Sprite { get; private set; }
        [field: SerializeField]
        public string TransferText { get; private set; }

        protected Panel panel;
        protected TransferPanel transferPanel;

        public virtual PointType Type => PointType.None;
        public int Index { get; private set; }

        public event Action<bool> OnChangeActivity;

        public void UpdateId(Guid id) => AssetId = id;

        public virtual void ShowPreview(Squad squad)
        {
            transferPanel.Initialize(this);
            if (!transferPanel.IsOpen)
                transferPanel.Open();
        }

        public virtual async Task Visit(Squad squad)
        {
            await squad.Visit(this);
        }

        public void SetIndex(int index) => Index = index;

        public void SetPanel(PanelManager panelManager, TransferPanel transferPanel)
        {
            panel = panelManager.GetPanelByPointInfo(Type);
            this.transferPanel = transferPanel;
            this.transferPanel.SetBattlePanel(panelManager.GetPanelByPointInfo(PointType.Battle));
        }

        public IEnumerable<PointModel> GetNextPoints()
        {
            if (!HasPort("exits"))
            {
                return new List<PointModel>();
            }

            NodePort port = GetPort("exits");
            List<NodePort> connections = port.GetConnections();

            List<PointModel> nextPoints = new();
            foreach (var connection in connections)
            {
                nextPoints.Add(connection.node is RandomNode randomNode ? randomNode.GetSelectedNextPoint() : connection.node as PointModel);
            }

            return nextPoints;
        }

        public void SetActive(bool isActive) => OnChangeActivity?.Invoke(isActive);

        private void OnValidate()
        {
            name = _name == "" ? GetType().Name : _name;
            if (AssetId == Guid.Empty)
                AssetId = Guid.NewGuid();
        }
    }
}