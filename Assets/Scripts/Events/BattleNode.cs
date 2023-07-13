using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;

namespace Events
{
    public class BattleNode : EventNode
    {
        [SerializeField]
        private List<CharacterTemplate> _enemies;
        
        public override void OnEnter(EventPanel panel)
        {
            panel.FinishEventWithBattle(_enemies);
        }
    }
}