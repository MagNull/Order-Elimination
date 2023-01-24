using System;
using CharacterAbility;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "CharacterInfo", menuName = "Character")]
    public class Character : ScriptableObject, IBattleCharacterInfo
    {
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
                UnmodifiedHealth = _strategyStats.HealthGrowth + _battleStats.UnmodifiedHealth,
                UnmodifiedArmor = _strategyStats.ArmorGrowth + _battleStats.UnmodifiedArmor,
                UnmodifiedAccuracy = _strategyStats.AccuracyGrowth + _battleStats.UnmodifiedAccuracy,
                UnmodifiedEvasion = _strategyStats.EvasionGrowth + _battleStats.UnmodifiedEvasion,
                UnmodifiedAttack = _strategyStats.AttackGrowth + _battleStats.UnmodifiedAttack
            };
            
            _battleStats = battleStats;
        }

        private void OnValidate()
        {
            _baseStats = _battleStats;
            _battleStats.UnmodifiedHealth = _battleStats.Health;
            _battleStats.UnmodifiedArmor = _battleStats.Armor;
            _battleStats.UnmodifiedAttack = _battleStats.Attack;
            _battleStats.UnmodifiedAccuracy = _battleStats.Accuracy;
            _battleStats.UnmodifiedEvasion = _battleStats.Evasion;
            _battleStats.UnmodifiedMovement = _battleStats.Movement;
        }
    }
}