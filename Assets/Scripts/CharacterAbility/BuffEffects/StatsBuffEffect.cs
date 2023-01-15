using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.BuffEffects
{
    public class StatsBuffEffect : TickEffectBase
    {
        public float Modifier { get; }
        public bool IsMultiplier { get; }
        private float _buffedValueAddition;
        public ScaleFromWhom ScaleFromWhom { get; }
        public Buff_Type StatType { get; }
        public IBattleObject Caster { get; }

        public StatsBuffEffect(Buff_Type statType, float modifier, ScaleFromWhom scaleFromWhom, int duration,
            bool isMultiplier, IBattleObject caster, ITickEffectView tickEffectView) : base(duration, tickEffectView)
        {
            StatType = statType;
            Modifier = modifier;
            ScaleFromWhom = scaleFromWhom;
            IsMultiplier = isMultiplier;
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
    }
}