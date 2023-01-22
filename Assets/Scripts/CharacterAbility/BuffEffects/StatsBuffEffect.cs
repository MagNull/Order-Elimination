using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.BuffEffects
{
    public class StatsBuffEffect : TickEffectBase
    {
        [SerializeField]
        private Buff_Type _statType;
        [SerializeField]
        private float _modifier;
        [SerializeField]
        private bool _isMultiplier;
        [SerializeField]
        private readonly ScaleFromWhom _scaleFromWhom;
        public float Modifier => _modifier;
        private float _buffedValueAddition;

        public bool IsMultiplier => _isMultiplier;

        public ScaleFromWhom ScaleFromWhom => _scaleFromWhom;

        public Buff_Type StatType => _statType;

        public IBattleObject Caster { get; }

        public StatsBuffEffect(bool isUnique, Buff_Type statType, float modifier, ScaleFromWhom scaleFromWhom,
            int duration,
            bool isMultiplier, IBattleObject caster, ITickEffectView tickEffectView) : base(duration, tickEffectView,
            isUnique)
        {
            _statType = statType;
            _modifier = modifier;
            _scaleFromWhom = scaleFromWhom;
            _isMultiplier = isMultiplier;
            Caster = caster;
        }

        public BattleStats Apply(IBuffTarget target)
        {
            var newStats = new BattleStats(target.Stats);
            var scaleTarget = ScaleFromWhom == ScaleFromWhom.Caster ? Caster : target;
            switch (StatType)
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
                    newStats.Health = GetModifiedValue(target.Stats.Health, scaleTarget.Stats.Health);
                    break;
                case Buff_Type.Movement:
                    newStats.Movement = GetModifiedValue(target.Stats.Movement, scaleTarget.Stats.Movement);
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
            switch (StatType)
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
            _buffedValueAddition = IsMultiplier ? scaleValue * (1 + Modifier) - value : Modifier;
            return Mathf.RoundToInt(IsMultiplier ? scaleValue * (1 + Modifier) : value + Modifier);
        }

        private int GetUnmodifiedValue(int value)
        {
            return Mathf.RoundToInt(value - _buffedValueAddition);
        }

        public override bool Equals(ITickEffect tickEffect)
        {
            return tickEffect is StatsBuffEffect statsBuffEffect && statsBuffEffect.StatType == StatType &&
                   Math.Abs(statsBuffEffect.Modifier - Modifier) < 0.01f &&
                   statsBuffEffect.IsMultiplier == IsMultiplier &&
                   statsBuffEffect.ScaleFromWhom == ScaleFromWhom &&
                   statsBuffEffect.StartDuration == StartDuration;
        }
    }
}