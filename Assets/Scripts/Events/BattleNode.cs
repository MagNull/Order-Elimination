using System.Collections.Generic;
using OrderElimination;
using OrderElimination.Events;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using XNode;

namespace Events
{
    public class BattleNode : Node, IEventNode
    {
        [Input]
        public Empty entries;
        
        [SerializeField]
        private List<CharacterTemplate> _enemies;
        
        public void Process(EventPanel panel, int index = 0)
        {
            Logging.LogError(new System.NotImplementedException());
        }

        public void OnEnter(EventPanel panel)
        {
            panel.FinishEventWithBattle(_enemies);
        }
    }
}