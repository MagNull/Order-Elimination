using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.UI;
using UnityEngine;
using VContainer;

namespace StartSessionMenu
{
    public class MetaShop : Panel
    {
        [SerializeField] 
        private MoneyCounter _uiCounter;
        [SerializeField]
        private List<UpgradeCategory> _progressCategories;
        [SerializeField] 
        private int StartMetaMoney = 1000;
        
        private Wallet _wallet;
        private ScenesMediator _mediator;
        
        [Inject]
        private void Construct(ScenesMediator scenesMediator)
        {
            _mediator = scenesMediator;
        }
        
        private void Start()
        {
            _wallet = new Wallet(StartMetaMoney);
            _uiCounter.Initialize(_wallet);
            foreach (var category in _progressCategories)
                category.OnUpgrade += Upgrade;
        }

        private void Upgrade(UpgradeCategory category)
        {
            var cost = category.TryUpgrade(_wallet.Money);
            if (cost < 0) return;
            _wallet.SubtractMoney(cost);
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
            _mediator.Register("stats", statsGrowth);
        }
    }
}