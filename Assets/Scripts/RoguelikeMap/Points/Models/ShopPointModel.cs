using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Inventory_Items;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class ShopPointModel : PointModel
    {
        [TabGroup("Items")]
        [SerializeField]
        private List<ItemData> _itemsData;
        [TabGroup("Costs")]
        [SerializeField] 
        private List<int> _itemsCost;
        
        public override PointType Type => PointType.Shop;
        public IReadOnlyList<int> Costs => _itemsCost;
        public IReadOnlyDictionary<ItemData, int> ItemsData => _itemsData.Zip(_itemsCost, ((data, cost) => new {data, cost} ))
            .ToDictionary(x => x.data, x => x.cost);
        public ShopPanel Panel => _panel as ShopPanel;

        public override void Visit(Squad squad)
        {
            base.Visit(squad);
            Panel.InitializeItems(ItemsData);
            Panel.Open();
        }
    }
}