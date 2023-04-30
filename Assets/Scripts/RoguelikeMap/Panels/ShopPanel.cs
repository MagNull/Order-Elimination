using System;
using System.Collections.Generic;
using System.Linq;
using Inventory_Items;
using OrderElimination;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using StartSessionMenu;
using UnityEngine;
using VContainer;

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
        
        private List<ItemData> _items;
        private static Wallet _wallet;
        
        private readonly List<ShopItem> _shopItems = new List<ShopItem>();
        
        [Inject]
        public void Wallet(Wallet wallet)
        {
            _wallet = wallet;
            _counter.Initialize(_wallet);
        }
        
        public override void SetInfo(PointModel model)
        {
            if(model is not ShopPointModel shopModel)
                throw new ArgumentException("Is not valid PointInfo");
            SetItems(shopModel.ItemsId);
        }

        private void SetItems(IReadOnlyList<ItemData> itemsId)
        {
            _items = LoadItems(itemsId);
        }

        private List<ItemData> LoadItems(IReadOnlyList<ItemData> items)
        {
            throw new NotImplementedException();
        }

        public void AddItems(List<FakeAbilityBase> abilityBases)
        {
            foreach (var ability in abilityBases)
            {
                var item = Instantiate(_itemPrefab, _shopList);
                item.Initialize(ability);
                _shopItems.Add(item);
            }
        }

        public void ClearShop()
        {
            foreach (var item in _shopItems)
                Destroy(item.gameObject);
            _shopItems.Clear();
        }

        public static void Buy(ShopItem item)
        {
            var abil = item.Ability;

            if (abil.Cost < _wallet.Money)
            {
                _wallet.SubtractMoney(abil.Cost);
                Debug.Log("Cost: " + item.Cost);
                item.gameObject.SetActive(false);
            }
        }

        public void OnDisable()
        {
            ClearShop();
        }

        // stub
        private void OnEnable()
        {
            var abilities = Resources.LoadAll<FakeAbility>("TestAbility");
            AddItems(abilities.Select(x => (FakeAbilityBase)x).ToList());
        }
    }
}