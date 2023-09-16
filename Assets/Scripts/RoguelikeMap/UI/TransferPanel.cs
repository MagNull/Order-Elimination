using System;
using RoguelikeMap.Points.Models;
using TMPro;
using UnityEngine;

namespace RoguelikeMap.UI
{
    public class TransferPanel : Panel
    {
        [SerializeField]
        private TMP_Text _text;

        private int _pointIndex;
        
        public event Action<int> OnAccept;

        public void Initialize(PointModel pointModel)
        {
            _text.text = pointModel.TransferText;
            _pointIndex = pointModel.Index;
            Open();
        }
        
        public void AcceptClick()
        {
            Close();
            OnAccept?.Invoke(_pointIndex);
        }
    }
}