using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public class ShopPointModel : PointModel
    {
        [SerializeField]
        private List<int> _itemsId;
        
        public override PointType Type => PointType.Shop;
        public IReadOnlyList<int> ItemsId => _itemsId;
    }
}