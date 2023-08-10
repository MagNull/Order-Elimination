using System;
using RoguelikeMap.Points;
using TMPro;
using UnityEngine;

namespace RoguelikeMap.UI
{
    public class TransferPanel : Panel
    {
        [SerializeField]
        private TMP_Text _text;
        private Point _point;
        
        public event Action<Point> OnAcceptClick;

        public void Initialize(Point point)
        {
            _point = point;
            _text.text = point.Model.TransferText;
            Open();
        }
        
        public void AcceptClick()
        {
            Close();
            OnAcceptClick?.Invoke(_point);
        }
    }
}