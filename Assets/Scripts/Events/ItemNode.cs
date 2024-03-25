using System.Collections.Generic;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using GameInventory.Items;


namespace Events
{
    enum ItemActionType
    {
        Remove,
        Add
    }

    [NodeWidth(300)]
    public class ItemNode : EventNode
    {
        [SerializeField]
        private ItemActionType _itemActionType;

        [SerializeField]
        private List<ItemData> _itemDatas;

        public override void OnEnter(EventPanel panel)
        {
            base.OnEnter(panel);
            switch (_itemActionType)
            {
                case ItemActionType.Add:
                    panel.AddItems(_itemDatas);
                    break;
                case ItemActionType:
                    panel.SpendItems(_itemDatas);
                    break;
            }
        }
    }
}