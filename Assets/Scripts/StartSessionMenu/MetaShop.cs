using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            foreach (var category in _progressCategories)
                category.OnUpgrade += Upgrade;
        }

        public void SetWallet(Wallet wallet)
        {
            _wallet = wallet;
        }

        private void Upgrade(UpgradeCategory category)
        {
            if (category.TryUpgrade(_wallet.Money))
                _wallet.SubtractMoney(category.CostOfUpgrade);
        }

        public void SaveStats()
        {
            var list = new List<int>(5);
            list.AddRange(_progressCategories.Select(category => category.ProgressCount * category.PercentInPart));
            SquadMediator.SetStatsCoefficient(list);
        }
    }
}