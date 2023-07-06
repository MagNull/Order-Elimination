using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Inventory_Items;
using Inventory.Items;
using OrderElimination;
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

        private Inventory_Items.Inventory _inventory;
        
        private readonly List<ShopItem> _items = new ();
        private Wallet _wallet;

        public event Action<IReadOnlyList<ItemData>> OnBuyItems;
        public event Action<bool> OnShopVisit;
        
        [Inject]
        public void Construct(Wallet wallet, Inventory_Items.Inventory inventory)
        {
            _wallet = wallet;
            _inventory = inventory;
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

        private void Buy(ShopItem shopItem)
        {
            if (shopItem.Cost >= _wallet.Money) 
                return;
            _wallet.SubtractMoney(shopItem.Cost);
            shopItem.Buy();
            var item = ItemFactory.Create(shopItem.Data);
            _inventory.AddItem(item);
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