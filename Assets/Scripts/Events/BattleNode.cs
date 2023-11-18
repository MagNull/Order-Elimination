using System.Collections.Generic;
using GameInventory.Items;
using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.Events;
using OrderElimination.MacroGame;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using UnityEngine.Rendering;

namespace Events
{
    [NodeWidth(300)]
    public class BattleNode : IEventNode
    {
        [Input]
        public Empty entries;
        
        [SerializeField]
        private List<CharacterTemplate> _enemies;
        [SerializeReference]
        private IBattleMapLayout _mapLayout;

        [field: SerializeField]
        public BattleRulesPreset BattleRules;
        [field: SerializeField]
        public SerializedDictionary<ItemData, float> ItemsDropProbability { get; private set; }
        public IBattleMapLayout MapLayout => _mapLayout;

        public IReadOnlyList<CharacterTemplate> Enemies => _enemies;

        public override void Process(EventPanel panel, int index = 0)
        {
            Logging.LogError(new System.NotImplementedException());
        }

        public override void OnEnter(EventPanel panel)
        {
            panel.FinishEventWithBattle(this);
        }
    }
}