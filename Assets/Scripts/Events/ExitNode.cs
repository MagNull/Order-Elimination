using System.Collections.Generic;
using GameInventory.Items;
using OrderElimination;
using OrderElimination.Events;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Events
{
    public class ExitNode : IEventNode
    {
        [Input]
        public Empty entries;

        [field: SerializeField]
        public bool IsHaveItems { get; private set; }
        
        [field: SerializeField]
        public bool IsAddMember { get; private set; }
        
        [SerializeField, ShowIf("IsHaveItems")]
        private List<ItemData> _itemsData;

        [SerializeField, ShowIf("IsAddMember")]
        private List<CharacterTemplate> _characters;

        public override void Process(EventPanel panel, int index = 0)
        {
            Logging.LogError(new System.NotImplementedException());
        }

        public override void OnEnter(EventPanel panel)
        {
            panel.FinishEvent(_itemsData, _characters);
        }
    }
}