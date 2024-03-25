using System;
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
        public static readonly string BuyedItemsKey = "BuyedItems";

        [SerializeField]
        private MoneyCounter _counter;
        [SerializeField]
        private ShopItem _itemPrefab;
        [SerializeField]
        private Transform _itemsParent;
        [SerializeField]
        private ItemInfoPanel _itemInfoPanel;

        private Inventory _inventory;

        private readonly List<ShopItem> _items = new();
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
            _itemInfoPanel.OnReturn += Return;
            foreach (var item in items)
            {
                var itemObject = Instantiate(_itemPrefab, _itemsParent);
                itemObject.Initialize(item.Data, item.Cost);
                itemObject.OnSelected += ShowItemInfo;
                _items.Add(itemObject);
            }
            UpdateItemState();
        }

        private void UpdateItemState()
        {
            if (!PlayerPrefs.HasKey(BuyedItemsKey))
            {
                return;
            }
            var buyedItems = PlayerPrefs.GetString(BuyedItemsKey);
            foreach (var itemId in buyedItems.Split())
            {
                foreach (var item in _items)
                {
                    if (item.Data.AssetId.ToString() == itemId)
                    {
                        item.Buy();
                    }
                }
            }
        }

        private void Buy()
        {
            if (_currentItem.Cost >= _wallet.Money)
                return;
            _wallet.Money -= _currentItem.Cost;
            _currentItem.Buy();
            var item = ItemFactory.Create(_currentItem.Data);
            _inventory.AddItem(item);
            AddBuyedItem();
        }

        private void AddBuyedItem()
        {
            string buyedItems = string.Empty;
            if (PlayerPrefs.HasKey(BuyedItemsKey))
            {
                buyedItems = PlayerPrefs.GetString(BuyedItemsKey) + ' ';
            }
            PlayerPrefs.SetString(BuyedItemsKey, buyedItems + _currentItem.Data.AssetId);
        }

        private void Return()
        {
            _wallet.Money += _currentItem.Cost;
            _currentItem.Return();
            _inventory.RemoveItem(_currentItem.Data);
            ReturnBuyedItem();
        }

        private void ReturnBuyedItem()
        {
            if (!PlayerPrefs.HasKey(BuyedItemsKey))
            {
                return;
            }
            var buyedItems = PlayerPrefs.GetString(BuyedItemsKey);
            var itemToRemove = _currentItem.Data.AssetId.ToString();
            int index = buyedItems.IndexOf(itemToRemove);
            string newBuyedItems = index < 0 ? buyedItems : buyedItems.Remove(index, itemToRemove.Length);
            PlayerPrefs.SetString(BuyedItemsKey, newBuyedItems);
        }

        private void ShowItemInfo(ShopItem item)
        {
            _itemInfoPanel.Initialize(item);
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
            PlayerPrefs.DeleteKey(BuyedItemsKey);
            base.Close();
        }

        public void OnDisable()
        {
            _itemInfoPanel.OnBuy -= Buy;
            _itemInfoPanel.OnReturn -= Return;
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