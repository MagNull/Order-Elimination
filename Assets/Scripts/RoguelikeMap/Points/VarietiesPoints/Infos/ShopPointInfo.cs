using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeMap.Points.VarietiesPoints.Infos
{
    [Serializable]
    public class ShopPointInfo : VarietiesPoint
    {
        [SerializeField]
        private List<int> _itemsId;
        
        public override PointType PointType => PointType.Shop;
        public IReadOnlyList<int> ItemsId => _itemsId;
    }
}