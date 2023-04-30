using System;
using System.Collections.Generic;
using Inventory_Items;
using OrderElimination;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using RoguelikeMap.Shop;
using StartSessionMenu;
using UnityEngine;

namespace RoguelikeMap.Panels
{
    public class ShopPanel : Panel
    {
        [SerializeField]
        private MoneyCounter _counter;
        [SerializeField]
        private ShopItem _itemPrefab;
        [SerializeField]
        private Transform _shopList;
        
        private readonly List<ShopItem> _items = new ();
        private Wallet _wallet;
        
        public void SetWallet(Wallet wallet)
        {
            _wallet = wallet;
            _counter.Initialize(_wallet);
        }
        
        public override void SetInfo(PointModel model)
        {
            if(model is not ShopPointModel shopModel)
                throw new ArgumentException("Is not valid PointInfo");
            InitializeItems(shopModel.ItemsData);
        }

        private void InitializeItems(IReadOnlyDictionary<ItemData, int> itemsData)
        {
            foreach (var data in itemsData)
            {
                var item = Instantiate(_itemPrefab, _shopList);
                item.Initialize(data.Key, data.Value);
                item.OnBuy += Buy;
                _items.Add(item);
            }
        }

        private void Buy(ShopItem item)
        {
            if (item.Cost >= _wallet.Money) 
                return;
            _wallet.SubtractMoney(item.Cost);
            item.Buy();
        }

        public void OnDisable()
        {
            ClearShop();
        }

        private void ClearShop()
        {
            foreach (var item in _items)
                Destroy(item.gameObject);
            _items.Clear();
        }
    }
}