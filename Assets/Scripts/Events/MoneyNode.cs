using System.Collections.Generic;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using GameInventory.Items;


namespace Events
{
    [NodeWidth(300)]
    public class MoneyNode : EventNode
    {
        [SerializeField]
        private int _money;

        public override void OnEnter(EventPanel panel)
        {
            base.OnEnter(panel);
            panel.AddMoney(_money);
        }
    }
}