using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameInventory.Items;
using OrderElimination;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.PointPanels;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace RoguelikeMap.Points.Models
{
    [Serializable]
    public struct ShopItemData
    {
        public ItemData Data;
        public int Cost;
        public int DropProbability;
    }
    
    [Serializable]
    public class ShopPointModel : PointModel
    {
        [Input] public PointModel entries;
        [Output] public PointModel exits;

        [SerializeField]
        private List<ShopItemData> _shopItems;

        public override PointType Type => PointType.Shop;
        public ShopPanel Panel => _panel as ShopPanel;

        private List<ShopItemData> RollItems()
        {
            var result = new List<ShopItemData>();
            for (var i = 0; i < _shopItems.Count; i++)
            {
                var roll = Random.Range(1, 100);
                if(roll <= _shopItems[i].DropProbability)
                    result.Add(_shopItems[i]);
            }

            return result;
        }

        public override async Task Visit(Squad squad)
        {
            await base.Visit(squad);
            Panel.InitializeItems(RollItems());
            Panel.Open();
        }
    }
}