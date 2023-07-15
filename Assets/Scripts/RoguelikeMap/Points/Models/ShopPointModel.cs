using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory.Items;
using OrderElimination;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class ShopPointModel : PointModel
    {
        [TabGroup("Items")]
        [SerializeField]
        private int _count;
        [SerializeField]
        private ItemsPool _itemsPool;
        [TabGroup("Costs")]
        [SerializeField] 
        private int _maxItemsCost;
        
        public override PointType Type => PointType.Shop;
        public IReadOnlyDictionary<ItemData, int> ItemsData => GetItems().Zip(GetCosts(), ((data, cost) => new {data, cost} ))
            .ToDictionary(x => x.data, x => x.cost);
        public ShopPanel Panel => _panel as ShopPanel;

        private List<ItemData> GetItems()
        {
            var result = new List<ItemData>();
            for (var i = 0; i < _count; i++)
            {
                var item = _itemsPool.GetRandomItem().Data;
                while(result.Contains(item))
                    item = _itemsPool.GetRandomItem().Data;
                result.Add(item);
            }

            return result;
        }

        private IReadOnlyList<int> GetCosts()
        {
            var result = new List<int>();
            for (var i = 0; i < _count; i++)
            {
                result.Add(Random.Range(100, _maxItemsCost + 1));
            }

            return result;
        }

        public override void Visit(Squad squad)
        {
            base.Visit(squad);
            Panel.InitializeItems(ItemsData);
            Panel.Open();
        }
    }
}