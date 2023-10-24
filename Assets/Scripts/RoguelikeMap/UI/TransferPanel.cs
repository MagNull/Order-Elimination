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

        private int _pointIndex;
        private Panel _battlePointTransferPanel;
        
        public event Action<int> OnAccept;

        public void Initialize(PointModel pointModel)
        {
            _text.text = pointModel.TransferText;
            _pointIndex = pointModel.Index;
        }
        
        public void AcceptClick()
        {
            Close();
            OnAccept?.Invoke(_pointIndex);
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