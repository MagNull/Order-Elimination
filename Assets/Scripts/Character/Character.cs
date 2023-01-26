using System;
using CharacterAbility;
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

        public void Awake()
        {
            Debug.Log("Awake");
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
        }
    }
}