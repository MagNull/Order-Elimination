using System;
using System.Collections.Generic;
using Inventory_Items;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class ShopPointModel : PointModel
    {
        [SerializeField]
        private List<ItemData> _itemsId;
        [SerializeField] 
        private List<int> _itemsCost;
        
        public override PointType Type => PointType.Shop;
        public IReadOnlyList<ItemData> ItemsId => _itemsId;
    }
}