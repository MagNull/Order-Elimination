using System;
using CharacterAbility;
using Inventory_Items;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "CharacterInfo", menuName = "Character")]
    public class Character : ScriptableObject, IBattleCharacterInfo
    {
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
        
        private readonly Inventory _inventory = new Inventory(3);

        public Inventory Inventory => _inventory;

        public IReadOnlyBattleStats GetBattleStats() => _battleStats;

        public string GetName() => _name;
        public Sprite GetViewIcon() => _viewIcon;
        public Sprite GetViewAvatar() => _viewAvatar;

        public AbilityInfo[] GetActiveAbilityInfos() => _activeAbilities;
        public AbilityInfo[] GetPassiveAbilityInfos() => _passiveAbilities;

        public StrategyStats GetStrategyStats() => _strategyStats;
        

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
            var battleStats = new BattleStats(_battleStats)
            {
                Health = _strategyStats.HealthGrowth * (1 + stats.HealthGrowth / 100),
                UnmodifiedHealth = _strategyStats.HealthGrowth * (1 + stats.HealthGrowth / 100),
                Armor = _strategyStats.ArmorGrowth * (1 + stats.ArmorGrowth / 100),
                UnmodifiedArmor = _strategyStats.ArmorGrowth * (1 + stats.ArmorGrowth / 100),
                Accuracy = _strategyStats.AccuracyGrowth * (1 + stats.AccuracyGrowth / 100),
                UnmodifiedAccuracy = _strategyStats.AccuracyGrowth * (1 + stats.AccuracyGrowth / 100),
                Evasion = _strategyStats.EvasionGrowth * (1 + stats.EvasionGrowth / 100),
                UnmodifiedEvasion = _strategyStats.EvasionGrowth * (1 + stats.EvasionGrowth / 100),
                Attack = _strategyStats.AttackGrowth * (1 + stats.AttackGrowth / 100),
                UnmodifiedAttack = _strategyStats.AttackGrowth * (1 + stats.AttackGrowth / 100)
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