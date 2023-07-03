using System;
using System.Linq;
using OrderElimination.AbilitySystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Conditions
{
    public interface ITargetCondition
    {
        public abstract bool Check(AbilitySystemActor target);
    }
    
    [Serializable]
    public class TargetHasPassive : ITargetCondition
    {
        [SerializeField]
        private PassiveAbilityBuilder[] _needPassiveEffects;

        [SerializeField]
        private bool _has;
        
        public bool Check(AbilitySystemActor target)
        {
            if (!_needPassiveEffects.Any())
                return _has;
            
            var targetPassives = target.PassiveAbilities.Select(ab => ab.AbilityData.BasedBuilder);
            if (_needPassiveEffects.All(ef => targetPassives.Contains(ef)))
            {
                return _has;
            }

            return !_has;
        }
    }
    
    [Serializable]
    public class TargetHasEffect : ITargetCondition
    {
        [FormerlySerializedAs("_needPassiveEffects")]
        [SerializeField]
        private EffectDataPreset[] _needEffects;

        [SerializeField]
        private bool _has;
        
        public bool Check(AbilitySystemActor target)
        {
            if (!_needEffects.Any())
                return _has;

            var targetEffects = target.Effects.Select(x => x.EffectData);
            if (_needEffects.All(ef => targetEffects.Contains(ef)))
            {
                return _has;
            }

            return !_has;
        }
    }
    
    enum Relation
    {
        Equal,
        Greater,
        Less
    }
    
    [Serializable]
    public class TargetHasStatValue : ITargetCondition
    {
        [SerializeField]
        private BattleStat _battleStat;

        [Range(0, 100)]
        [SerializeField]
        private float _targetValuePercentage;

        [SerializeField]
        private Relation _relation;
        public bool Check(AbilitySystemActor target)
        {
            switch (_battleStat)
            {
                case BattleStat.MaxHealth:
                    return Compare(target.LifeStats.Health, target.BattleStats[_battleStat].UnmodifiedValue);
                case BattleStat.MaxArmor:
                    return Compare(target.LifeStats.TotalArmor, target.BattleStats[_battleStat].UnmodifiedValue);
                default:
                    return Compare(target.BattleStats[_battleStat].ModifiedValue, 100);
            }
        }

        private bool Compare(float value, float maxValue)
        {
            switch (_relation)
            {
                case Relation.Equal:
                    return value == _targetValuePercentage * (maxValue / 100);
                case Relation.Greater:
                    return value > _targetValuePercentage * (maxValue / 100);
                case Relation.Less:
                    return value < _targetValuePercentage * (maxValue / 100);
                default:
                    return false;
            }
        }
    }
}