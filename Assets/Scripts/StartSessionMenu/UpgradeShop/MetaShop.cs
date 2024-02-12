using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using OrderElimination.SavesManagement;
using RoguelikeMap.UI;
using Sirenix.OdinInspector;
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
        [ShowInInspector]
        private IStatUpgradeRules _upgradeRules;
        private Wallet _wallet;
        private Dictionary<BattleStat, UpgradeCategory> _categoriesByStats;
        private Dictionary<UpgradeCategory, BattleStat> _statsByCategories;
        private Dictionary<BattleStat, int> _upgradeLevels;

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

        private StatModifiers UpgradeStats
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
            _playerProgressManager = scenesMediator.Get<IPlayerProgressManager>(MediatorRegistration.ProgressManager);
        }

        private void Start()
        {
            var availableStats = new BattleStat[] { 
                BattleStat.MaxHealth, 
                BattleStat.AttackDamage, 
                BattleStat.MaxArmor, 
                BattleStat.Evasion,
                BattleStat.Accuracy};
            _wallet = new Wallet(
                () => UpgradeMoney, 
                value => UpgradeMoney = value);
            _uiCounter.Initialize(_wallet);
            _upgradeRules ??= new StatUpgradeRules();
            _categoriesByStats = new()
            {
                { BattleStat.MaxHealth, _progressCategories[0] },
                { BattleStat.AttackDamage, _progressCategories[1] },
                { BattleStat.MaxArmor, _progressCategories[2] },
                { BattleStat.Evasion, _progressCategories[3] },
                { BattleStat.Accuracy, _progressCategories[4] },
            };
            _statsByCategories = _categoriesByStats.ToDictionary(kv => kv.Value, kv => kv.Key);
            _upgradeLevels = availableStats.ToDictionary(s => s, s => 0);
            foreach (var category in _progressCategories)
            {
                category.UpgradeButtonClicked += OnUpgradeAttempt;
                category.MaxUpgradeLevel = (int)_upgradeRules.MaxUpgradeLevel;
            }
            RestoreUpgrades(UpgradeStats);
        }

        private void OnUpgradeAttempt(UpgradeCategory category)
        {
            var stat = _statsByCategories[category];
            var currentLevel = _upgradeLevels[stat];
            var nextLevel = currentLevel + 1;
            var upgradeCost = Mathf.FloorToInt(
                _upgradeRules.GetUpgradeToLevelCost(stat, nextLevel));
            if (nextLevel <= _upgradeRules.MaxUpgradeLevel
                && upgradeCost <= UpgradeMoney)
            {
                _upgradeLevels[stat] = nextLevel;
                UpgradeStats = ModifyUpgradeStats(UpgradeStats);
                //TODO: Leave only 1st or 2nd option
                _wallet.Money -= upgradeCost;

                UpdateStatCategoryView(category);
            }
        }

        private void UpdateStatCategoryView(UpgradeCategory category)
        {
            var stat = _statsByCategories[category];
            var currentLevel = _upgradeLevels[stat];
            var nextLevel = currentLevel + 1;
            var upgradeCost = _upgradeRules.GetUpgradeToLevelCost(stat, nextLevel);
            var modifier = _upgradeRules.GetStatModifier(stat, currentLevel);
            var operation = modifier.Operation;
            float upgradeIncrease;

            if (operation == BinaryMathOperation.Add || operation == BinaryMathOperation.Subtract)
                upgradeIncrease = modifier.Operand * 100;
            else if (operation == BinaryMathOperation.Multiply)
                upgradeIncrease = (modifier.Operand - 1) * 100;
            else
                throw new NotImplementedException();

            category.ProgressLevel = Mathf.RoundToInt(currentLevel);
            category.IncreaseAmount = Mathf.RoundToInt(upgradeIncrease);
            category.CostOfUpgrade = Mathf.RoundToInt(upgradeCost);
        }

        private StatModifiers ModifyUpgradeStats(StatModifiers stats)
        {
            foreach (var stat in _categoriesByStats.Keys)
            {
                var currentLevel = _upgradeLevels[stat];
                stats.SetModifier(stat, _upgradeRules.GetStatModifier(stat, currentLevel));
            }
            return stats;
        }

        private void RestoreUpgrades(StatModifiers stats)
        {
            foreach (var stat in _categoriesByStats.Keys)
            {
                var category = _categoriesByStats[stat];
                if (stats.Modifiers.ContainsKey(stat))
                {
                    var modifier = stats.Modifiers[stat];
                    var level = _upgradeRules.GetEstimatedUpgradeLevel(stat, modifier);
                    _upgradeLevels[stat] = Mathf.RoundToInt(level);
                }
                UpdateStatCategoryView(category);
            }
        }
    }
}