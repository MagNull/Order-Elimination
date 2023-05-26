using System;
using CharacterAbility;
using Inventory_Items;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using Sirenix.Serialization;
using OrderElimination.Domain;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "CharacterInfo", menuName = "Character")]
    public class Character : SerializedScriptableObject, IBattleCharacterInfo, IBattleCharacterData
    {
        //New System
        [OdinSerialize, ShowInInspector]
        private ReadOnlyBaseStats _baseBattleStats;
        [SerializeField]
        private EntityType _entityType;
        [SerializeReference]
        private ActiveAbilityBuilder[] _activeAbilitiesData;
        [SerializeReference]
        private ActiveAbilityBuilder[] _passiveAbilitiesData;

        public ReadOnlyBaseStats BaseStats => _baseBattleStats;
        //public EntityType EntityType => _entityType;
        public ActiveAbilityBuilder[] GetActiveAbilities() => _activeAbilitiesData;
        public ActiveAbilityBuilder[] GetPassiveAbilities() => _passiveAbilitiesData;
        //

        [SerializeField]
        private BattleStats _baseStats;
        [SerializeField]
        private BattleStats _battleStats;
        [SerializeField]
        private StrategyStats _strategyStats;
        [SerializeField]
        private string _name;
        [SerializeField]
        private Sprite _viewIcon;
        [SerializeField]
        private Sprite _viewAvatar;
        [FormerlySerializedAs("_abilities")]
        [SerializeField]
        private AbilityInfo[] _activeAbilities;
        [SerializeField]
        private AbilityInfo[] _passiveAbilities;
        
        [ShowInInspector]
        private Inventory_Items.Inventory _inventory = new Inventory_Items.Inventory(2);

        public Inventory_Items.Inventory Inventory => _inventory;

        public IReadOnlyBattleStats GetBattleStats() => _battleStats;

        public string Name => _name;
        public Sprite BattleIcon => _viewIcon;
        public Sprite Avatar => _viewAvatar;

        public AbilityInfo[] GetActiveAbilityInfos() => _activeAbilities;
        public AbilityInfo[] GetPassiveAbilityInfos() => _passiveAbilities;

        public StrategyStats GetStrategyStats() => _strategyStats;


        [Button]
        public void ResetInventory()
        {
            _inventory = new Inventory_Items.Inventory(2);
        }

        public void RaiseExperience(float experience)
        {
            throw new System.NotImplementedException();
        }

        public void SetLevel(int level)
        {
            if (_strategyStats.Lvl == level)
                return;
            for(var i = _strategyStats.Lvl; i <= level; i++)
                Upgrade();
        }

        public void Heal(int healStat)
        {
            _battleStats.Health += healStat;
        }

        public void Upgrade(StrategyStats stats)
        {
            var battleStats = new BattleStats()
            {
                UnmodifiedHealth = _battleStats.UnmodifiedHealth + _strategyStats.HealthGrowth * stats.HealthGrowth,
                UnmodifiedArmor = _battleStats.UnmodifiedArmor + _strategyStats.ArmorGrowth * stats.ArmorGrowth,
                UnmodifiedAccuracy = _battleStats.UnmodifiedAccuracy + _strategyStats.AccuracyGrowth * stats.AccuracyGrowth,
                UnmodifiedEvasion = _battleStats.UnmodifiedEvasion + _strategyStats.EvasionGrowth * stats.EvasionGrowth,
                UnmodifiedAttack = _battleStats.UnmodifiedAttack + _strategyStats.AttackGrowth * stats.AttackGrowth,
                Health = _battleStats.Health + _strategyStats.HealthGrowth * stats.HealthGrowth,
                Armor = _battleStats.Armor + _strategyStats.ArmorGrowth * stats.ArmorGrowth,
                Accuracy = _battleStats.Accuracy + _strategyStats.AccuracyGrowth * stats.AccuracyGrowth,
                Evasion = _battleStats.Evasion + _strategyStats.EvasionGrowth * stats.EvasionGrowth,
                Attack = _battleStats.Attack + _strategyStats.AttackGrowth * stats.AttackGrowth,
            };

            _battleStats = battleStats;
        }
        
        public void Upgrade()
        {
            var battleStats = new BattleStats(_battleStats)
            {
                Health = _strategyStats.HealthGrowth + _battleStats.UnmodifiedHealth,
                UnmodifiedHealth = _strategyStats.HealthGrowth + _battleStats.UnmodifiedHealth,
                Armor = _strategyStats.ArmorGrowth + _battleStats.UnmodifiedArmor,
                UnmodifiedArmor = _strategyStats.ArmorGrowth + _battleStats.UnmodifiedArmor,
                Accuracy = _strategyStats.AccuracyGrowth + _battleStats.UnmodifiedAccuracy,
                UnmodifiedAccuracy = _strategyStats.AccuracyGrowth + _battleStats.UnmodifiedAccuracy,
                Evasion = _strategyStats.EvasionGrowth + _battleStats.UnmodifiedEvasion,
                UnmodifiedEvasion = _strategyStats.EvasionGrowth + _battleStats.UnmodifiedEvasion,
                Attack = _strategyStats.AttackGrowth + _battleStats.UnmodifiedAttack,
                UnmodifiedAttack = _strategyStats.AttackGrowth + _battleStats.UnmodifiedAttack
            };
            _strategyStats.Lvl++;

            Debug.Log($"Health: Old - {_battleStats.UnmodifiedHealth}, New - {battleStats.UnmodifiedHealth}");
            Debug.Log($"Health: Old - {_battleStats.UnmodifiedArmor}, New - {battleStats.UnmodifiedArmor}");
            Debug.Log($"Health: Old - {_battleStats.UnmodifiedAccuracy}, New - {battleStats.UnmodifiedAccuracy}");
            Debug.Log($"Health: Old - {_battleStats.UnmodifiedEvasion}, New - {battleStats.UnmodifiedEvasion}");
            Debug.Log($"Health: Old - {_battleStats.UnmodifiedAttack}, New - {battleStats.UnmodifiedAttack}");
            _battleStats = battleStats;
        }

        private void OnValidate()
        {
            _battleStats.UnmodifiedHealth = _battleStats.Health;
            _battleStats.UnmodifiedArmor = _battleStats.Armor;
            _battleStats.UnmodifiedAttack = _battleStats.Attack;
            _battleStats.UnmodifiedAccuracy = _battleStats.Accuracy;
            _battleStats.UnmodifiedEvasion = _battleStats.Evasion;
            _battleStats.UnmodifiedMovement = _battleStats.Movement;
        }

        [Button]
        public void ResetStats()
        {
            _battleStats = _baseStats;
            _strategyStats.Lvl = 1;
        }
    }
}