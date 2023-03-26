using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class MetaShop : MonoBehaviour
    {
        [SerializeField]
        private List<UpgradeCategory> _progressCategories;
        
        private Wallet _wallet;
        private int Money = 1000;
        
        private void Start()
        {
            _wallet = new Wallet(Money);
            foreach (var category in _progressCategories)
                category.OnUpgrade += Upgrade;
        }

        private void Upgrade(UpgradeCategory category)
        {
            if (category.TryUpgrade(_wallet.Money))
                _wallet.SubtractMoney(category.CostOfUpgrade);
        }
    }
}