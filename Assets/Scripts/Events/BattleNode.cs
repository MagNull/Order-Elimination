using System.Collections.Generic;
using GameInventory.Items;
using OrderElimination;
using OrderElimination.Events;
using OrderElimination.MacroGame;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using UnityEngine.Rendering;
using XNode;

namespace Events
{
    [NodeWidth(300)]
    public class BattleNode : Node, IEventNode
    {
        [Input]
        public Empty entries;
        
        [SerializeField]
        private List<CharacterTemplate> _enemies;

        [field: SerializeField] 
        public BattleScenario Scenario { get; private set; }
        [field: SerializeField]
        public SerializedDictionary<ItemData, float> ItemsDropProbability { get; private set; }

        public IReadOnlyList<CharacterTemplate> Enemies => _enemies;

        public void Process(EventPanel panel, int index = 0)
        {
            Logging.LogError(new System.NotImplementedException());
        }

        public void OnEnter(EventPanel panel)
        {
            panel.FinishEventWithBattle(this);
        }
    }
}