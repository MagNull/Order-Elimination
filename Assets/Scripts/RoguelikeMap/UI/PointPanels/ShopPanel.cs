﻿using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using OrderElimination;
using RoguelikeMap.Panels;
using RoguelikeMap.Shop;
using StartSessionMenu;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.UI.PointPanels
{
    public class ShopPanel : Panel
    {
        [SerializeField]
        private MoneyCounter _counter;
        [SerializeField]
        private ShopItem _itemPrefab;
        [SerializeField]
        private Transform _itemsParent;
        
        private readonly List<ShopItem> _items = new ();
        private Wallet _wallet;

        public event Action<IReadOnlyList<ItemData>> OnBuyItems;
        
        [Inject]
        public void Construct(Wallet wallet)
        {
            _wallet = wallet;
            _counter.Initialize(_wallet);
        }
        
        public void InitializeItems(IReadOnlyDictionary<ItemData, int> itemsData)
        {
            foreach (var data in itemsData)
            {
                var item = Instantiate(_itemPrefab, _itemsParent);
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

        public override void Close()
        {
            OnBuyItems?.Invoke(_items.Where(x => x.IsBuy).Select(x => x.Data).ToList());
            base.Close();
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