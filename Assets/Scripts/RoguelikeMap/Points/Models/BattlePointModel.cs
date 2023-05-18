using System;
using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class BattlePointModel : PointModel
    {
        [SerializeField]
        private List<Character> _enemies;
        [SerializeField]
        private int _mapNumber;
        
        protected BattlePanel Panel => _panel as BattlePanel;
        
        public override PointType Type => PointType.Battle;
        public IReadOnlyList<IBattleCharacterInfo> Enemies => _enemies;
        public int MapNumber => _mapNumber;

        public override void Visit(Squad squad)
        {
            base.Visit(squad);
            Panel.UpdateEnemies(Enemies);
            Panel.Open();
        }
    }
}