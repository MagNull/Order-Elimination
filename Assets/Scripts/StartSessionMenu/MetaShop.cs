using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.AbilitySystem;
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
            var statsGrowth = new StrategyStats()
            {
                HealthGrowth = _progressCategories[0].ProgressCount * _progressCategories[0].PercentInPart,
                AttackGrowth = _progressCategories[1].ProgressCount * _progressCategories[1].PercentInPart,
                ArmorGrowth = _progressCategories[2].ProgressCount * _progressCategories[2].PercentInPart,
                EvasionGrowth = _progressCategories[3].ProgressCount * _progressCategories[3].PercentInPart,
                AccuracyGrowth = _progressCategories[4].ProgressCount * _progressCategories[4].PercentInPart
            };
            SquadMediator.SetStatsCoefficient(statsGrowth);
        }
    }
}