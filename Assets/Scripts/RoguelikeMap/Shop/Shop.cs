using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using StartSessionMenu;
using UnityEngine;
using VContainer;

namespace OrderElimination
{
    public class Shop : MonoBehaviour
    {
        [SerializeField]
        private MoneyCounter _counter;
        [SerializeField]
        private ShopItem _itemPrefab;
        [SerializeField]
        private Transform _shopList;

        private static Wallet _wallet;
        private readonly List<ShopItem> _shopItems = new List<ShopItem>();

        [Inject]
        public void Wallet(Wallet wallet)
        {
            _wallet = wallet;
            _counter.Initialize(_wallet);
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

        public void AddItems(FakeAbilityBase ability)
        {
            var item = Instantiate(_itemPrefab, transform);
            item.Initialize(ability);
            _shopItems.Add(item);
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
            AddItems(abilities.Select(x => (FakeAbilityBase) x).ToList());
        }
    }
}