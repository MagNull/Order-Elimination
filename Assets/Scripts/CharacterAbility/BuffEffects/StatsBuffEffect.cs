using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.BuffEffects
{
    public class StatsBuffEffect : TickEffectBase
    {
        private readonly Buff_Type _statType;
        [SerializeField]
        private float _modifier;
        [SerializeField]
        private bool _isMultiplier;
        private readonly IBattleObject _caster;
        [SerializeField]
        private float _modificator;
        [SerializeField]
        private ScaleFromWhom _scaleFromWhom;

        public StatsBuffEffect(Buff_Type statType, float modifier, ScaleFromWhom scaleFromWhom, int duration,
            bool isMultiplier, IBattleObject caster, ITickEffectView tickEffectView) : base(duration, tickEffectView)
        {
            _statType = statType;
            _modifier = modifier;
            _scaleFromWhom = scaleFromWhom;
            _isMultiplier = isMultiplier;
            _caster = caster;
        }

        public BattleStats Apply(IBuffTarget target)
        {
            var newStats = new BattleStats(target.Stats);
            var scaleTarget = _scaleFromWhom == ScaleFromWhom.Caster ? _caster : target;
            switch (_statType)
            {
                case Buff_Type.Accuracy:
                    newStats.Accuracy = GetModifiedValue(target.Stats.Accuracy, scaleTarget.Stats.Accuracy);
                    break;
                case Buff_Type.Attack:
                    newStats.Attack = GetModifiedValue(target.Stats.Attack, scaleTarget.Stats.Attack);
                    break;
                case Buff_Type.Evasion:
                    newStats.Evasion = GetModifiedValue(target.Stats.Evasion, scaleTarget.Stats.Evasion);
                    break;
                case Buff_Type.Health:
                    newStats.Health = GetModifiedValue(target.Stats.Evasion, scaleTarget.Stats.Evasion);
                    break;
                case Buff_Type.Movement:
                    newStats.Movement = GetModifiedValue(target.Stats.Evasion, scaleTarget.Stats.Evasion);
                    break;
                case Buff_Type.AdditionalArmor:
                    newStats.AdditionalArmor =
                        GetModifiedValue(target.Stats.AdditionalArmor, scaleTarget.Stats.UnmodifiedArmor) -
                        scaleTarget.Stats.UnmodifiedArmor;
                    break;
            }

            return newStats;
        }

        public BattleStats Remove(IBuffTarget target)
        {
            var newStats = new BattleStats(target.Stats);
            switch (_statType)
            {
                case Buff_Type.Accuracy:
                    newStats.Accuracy = GetUnmodifiedValue(target.Stats.Accuracy);
                    break;
                case Buff_Type.Attack:
                    newStats.Attack = GetUnmodifiedValue(target.Stats.Attack);
                    break;
                case Buff_Type.Evasion:
                    newStats.Evasion = GetUnmodifiedValue(target.Stats.Evasion);
                    break;
                case Buff_Type.Health:
                    newStats.Health = GetUnmodifiedValue(target.Stats.Health);
                    break;
                case Buff_Type.Movement:
                    newStats.Movement = GetUnmodifiedValue(target.Stats.Movement);
                    break;
                case Buff_Type.AdditionalArmor:
                    newStats.AdditionalArmor = GetUnmodifiedValue(target.Stats.AdditionalArmor);
                    break;
            }

            return newStats;
        }

        private int GetModifiedValue(int value, int scaleValue)
        {
            _modificator = _isMultiplier ? scaleValue * (1 + _modifier) - value : _modifier;
            return Mathf.RoundToInt(_isMultiplier ? scaleValue * (1 + _modifier) : value + _modifier);
        }

        private int GetUnmodifiedValue(int value)
        {
            return Mathf.RoundToInt(value - _modificator);
        }
    }
}