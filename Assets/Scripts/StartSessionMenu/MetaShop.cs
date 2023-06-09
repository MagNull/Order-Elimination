using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using RoguelikeMap.UI;
using UnityEngine;
using VContainer;

namespace StartSessionMenu
{
    public class MetaShop : Panel
    {
        [SerializeField]
        private List<UpgradeCategory> _progressCategories;
        
        private Wallet _wallet;

        [Inject]
        public void Configure(Wallet wallet)
        {
            _wallet = wallet;
        }
        
        private void Start()
        {
            foreach (var category in _progressCategories)
                category.OnUpgrade += Upgrade;
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