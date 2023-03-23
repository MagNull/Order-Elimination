using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using UnityEngine;
using VContainer;

namespace OrderElimination
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] 
        private ShopPage _pagePrefab;
        [SerializeField]
        private MoneyCounter _counter;
        [SerializeField] 
        private PageButton _buttonPrefab;
        [SerializeField] 
        private Transform _buttonsTransform;

        private List<ShopPage> _pages = new List<ShopPage>();
        
        private static Wallet _wallet;
        [Inject]
        public void Wallet(Wallet wallet)
        {
            _wallet = wallet;
            _counter.Initialize(_wallet);
            print(wallet.Money);
        }

        public void AddShopPage(string pageName, List<FakeAbilityBase> abilityBases)
        {
            var page = Instantiate(_pagePrefab, transform);
            page.CreateItems(abilityBases);
            _pages.Add(page);
        
            var button = Instantiate(_buttonPrefab, _buttonsTransform);
            button.Initialize(page, pageName);
        }

        public static void Buy(ShopItem item)
        {
            var abil = item.Ability;

            if (abil.Cost < _wallet.Money)
            {
                _wallet.SubtractMoney(abil.Cost);
                Debug.Log("Cost: " + item.Cost);
                Destroy(item.gameObject);   
            }
        }
    
        // stub
        private void Start()
        {
            var abilities = Resources.LoadAll<FakeAbility>("TestAbility");
            AddShopPage("Предметы", abilities.Select(x=>(FakeAbilityBase)x).ToList());
            AddShopPage("Абилки", abilities.Select(x=>(FakeAbilityBase)x).Reverse().ToList());
        }
    }
}
