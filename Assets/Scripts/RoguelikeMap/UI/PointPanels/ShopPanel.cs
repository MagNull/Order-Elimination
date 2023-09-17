﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory;
using GameInventory.Items;
using OrderElimination;
using RoguelikeMap.Points.Models;
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
        [SerializeField]
        private ItemInfoPanel _itemInfoPanel;

        private Inventory _inventory;
        
        private readonly List<ShopItem> _items = new ();
        private Wallet _wallet;
        private ShopItem _currentItem;

        public event Action<IReadOnlyList<ItemData>> OnBuyItems;
        public event Action<bool> OnShopVisit;
        
        [Inject]
        public void Construct(Wallet wallet, Inventory inventory)
        {
            _wallet = wallet;
            _inventory = inventory;
            _counter.Initialize(_wallet);
        }
        
        public void InitializeItems(IReadOnlyList<ShopItemData> items)
        {
            _itemInfoPanel.OnBuy += Buy;
            foreach (var item in items)
            {
                var itemObject = Instantiate(_itemPrefab, _itemsParent);
                itemObject.Initialize(item.Data, item.Cost);
                itemObject.OnSelected += ShowItemInfo;
                _items.Add(itemObject);
            }
        }

        private void Buy()
        {
            if (_currentItem.Cost >= _wallet.Money) 
                return;
            _wallet.SubtractMoney(_currentItem.Cost);
            _currentItem.Buy();
            var item = ItemFactory.Create(_currentItem.Data);
            _inventory.AddItem(item);
        }

        private void ShowItemInfo(ShopItem item)
        {
            _itemInfoPanel.Initialize(item.Data.View);
            _itemInfoPanel.Open();
            _currentItem = item;
        }

        public override void Open()
        {
            OnShopVisit?.Invoke(true);
            base.Open();
        }

        public override void Close()
        {
            OnBuyItems?.Invoke(_items.Where(x => x.IsBuy).Select(x => x.Data).ToList());
            OnShopVisit?.Invoke(false);
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