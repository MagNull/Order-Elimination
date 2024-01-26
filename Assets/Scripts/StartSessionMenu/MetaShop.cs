using System;
using System.Collections.Generic;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.SavesManagement;
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
        private Wallet _wallet;

        private IPlayerProgressManager _playerProgressManager;
        private IPlayerProgress PlayerProgress => _playerProgressManager.GetPlayerProgress();

        private int UpgradeMoney
        {
            get => PlayerProgress.MetaProgress.MetaCurrency;
            set
            {
                PlayerProgress.MetaProgress.MetaCurrency = value;
            }
        }

        private UpgradeStats UpgradeStats
        {
            get => PlayerProgress.MetaProgress.StatUpgrades;
            set
            {
                PlayerProgress.MetaProgress.StatUpgrades = value;
            }
        }

        [Inject]
        private void Construct(ScenesMediator scenesMediator)
        {
            _playerProgressManager = scenesMediator.Get<IPlayerProgressManager>("progress manager");
        }
        
        private void Start()
        {
            _wallet = new Wallet(
                () => UpgradeMoney, 
                value => UpgradeMoney = value);
            _uiCounter.Initialize(_wallet);
            foreach (var category in _progressCategories)
                category.OnUpgrade += Upgrade;
            RestoreUpgrades(UpgradeStats);
        }

        private void Upgrade(UpgradeCategory category)
        {
            var money = UpgradeMoney;
            var cost = category.CostOfUpgrade;
            if (category.CanUpgrade(money))
            {
                category.Upgrade(money);
                UpgradeStats = CalculateUpgradeStats();
                //TODO: Leave only 1st or 2nd option
                UpgradeMoney -= cost;
                _wallet.Money -= cost;
            }
        }

        private UpgradeStats CalculateUpgradeStats()
        {
            var statsGrowth = new UpgradeStats()
            {
                HealthGrowth = _progressCategories[0].ProgressCount * _progressCategories[0].PercentInPart,
                AttackGrowth = _progressCategories[1].ProgressCount * _progressCategories[1].PercentInPart,
                ArmorGrowth = _progressCategories[2].ProgressCount * _progressCategories[2].PercentInPart,
                EvasionGrowth = _progressCategories[3].ProgressCount * _progressCategories[3].PercentInPart,
                AccuracyGrowth = _progressCategories[4].ProgressCount * _progressCategories[4].PercentInPart
            };
            return statsGrowth;
        }

        private void RestoreUpgrades(UpgradeStats stats)
        {
            _progressCategories[0].ProgressCount = stats.HealthGrowth / _progressCategories[0].PercentInPart;
            _progressCategories[1].ProgressCount = stats.AttackGrowth / _progressCategories[1].PercentInPart;
            _progressCategories[2].ProgressCount = stats.ArmorGrowth / _progressCategories[2].PercentInPart;
            _progressCategories[3].ProgressCount = stats.EvasionGrowth / _progressCategories[3].PercentInPart;
            _progressCategories[4].ProgressCount = stats.AccuracyGrowth / _progressCategories[4].PercentInPart;
        }
    }
}