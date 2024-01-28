using System;
using RoguelikeMap.Points.Models;
using RoguelikeMap.UI.PointPanels;
using TMPro;
using UnityEngine;

namespace RoguelikeMap.UI
{
    public class TransferPanel : Panel
    {
        [SerializeField]
        private TMP_Text _text;

        private Guid _pointId;
        private Panel _battlePointTransferPanel;
        
        public event Action<Guid> OnAccept;

        public void Initialize(PointModel pointModel)
        {
            _text.text = pointModel.TransferText;
            _pointId = pointModel.AssetId;
        }
        
        public void AcceptClick()
        {
            Close();
            OnAccept?.Invoke(_pointId);
        }

        public void SetBattlePanel(Panel panel)
        {
            _battlePointTransferPanel = panel;
        }

        public override void Open()
        {
            if(_battlePointTransferPanel.IsOpen) 
                _battlePointTransferPanel.Close();
            base.Open();
        }
    }
}