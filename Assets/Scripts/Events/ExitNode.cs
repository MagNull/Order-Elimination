using System.Collections.Generic;
using GameInventory.Items;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Events
{
    public class ExitNode : EventNode
    {
        [field: SerializeField]
        public bool IsHaveItems { get; private set; }
        
        [SerializeField, ShowIf("IsHaveItems")]
        private List<ItemData> _itemsData;

        // public override void Process(EventPanel panel, int index = 0)
        // {
        //     panel.FinishEvent(_itemsData);
        // }

        public override void OnEnter(EventPanel panel)
        {
            panel.FinishEvent(_itemsData);
        }
    }
}