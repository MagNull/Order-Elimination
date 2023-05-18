using System;
using System.Collections.Generic;
using Inventory;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class SafeZonePointModel : PointModel
    {
        [SerializeField] 
        private Sprite _sprite;
        [SerializeField] 
        private string _text;
        [SerializeField] 
        private int _amountHeal;
        [SerializeField] 
        private List<ItemData> _items;

        public Sprite Sprite => _sprite;
        public string Text => _text;
        public int AmountHeal => _amountHeal;
        public IReadOnlyList<ItemData> Items => _items;
        public override PointType Type => PointType.SafeZone;
        public SafeZonePanel Panel => _panel as SafeZonePanel;
        
        public override void Visit(Squad squad)
        {
            base.Visit(squad);
            Panel.SetInfo(_amountHeal, _items, _sprite, _text);
            Panel.Open();
        }
    }
}