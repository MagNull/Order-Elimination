using CharacterAbility.BuffEffects;
using OrderElimination;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;

namespace CharacterAbility
{
    //public class EffectParameterAttribute : Attribute
    //{
    //    public readonly string ParameterKey;
    //    public readonly ValueUnits Units;

    //    public EffectParameterAttribute(string parameterKey, ValueUnits units)
    //    {
    //        ParameterKey = parameterKey;
    //        Units = units;
    //    }
    //}

    public enum ValueUnits
    {
        None,
        Percents,
        Cells,
        Turns
    }

    public interface ITickEffectView
    {
        string EffectName { get; }
        Sprite EffectIcon { get; }
        //IReadOnlyDictionary<string, string> GetDisplayableParameters(IReadOnlyBattleStats characterStats);
        bool DisplayAsMainEffect { get; }
        bool DisplayWhenApplied { get; }
    }

    [Serializable]
    public class EffectView : ITickEffectView
    {
        [SerializeField, ShowIf("@!DisplayAsMainEffect || DisplayWhenApplied")]
        private string _effectName;
        [SerializeField, ShowIf("@!DisplayAsMainEffect || DisplayWhenApplied")]
        private Sprite _effectIcon;
        [SerializeField]
        private bool _displayAsMainEffect = false;
        [SerializeField]
        private bool _displayWhenApplied = false;
        public string EffectName => _effectName;
        public Sprite EffectIcon => _effectIcon;
        //private AbilityEffect _effectModel;
        //public IReadOnlyDictionary<string, string> GetDisplayableParameters(IReadOnlyBattleStats characterStats)
        //    => _effectModel.GetDisplayableParameters(characterStats) ?? throw new InvalidOperationException();
        public bool DisplayAsMainEffect => _displayAsMainEffect;
        public bool DisplayWhenApplied => _displayWhenApplied;

        public EffectView(Sprite icon, string name)
        {
            _effectIcon = icon;
            _effectName = name;
        }

        private static readonly Dictionary<Buff_Type, ValueUnits> _buffUnits = new Dictionary<Buff_Type, ValueUnits>()
        {
            {Buff_Type.Attack, ValueUnits.None }, 
            {Buff_Type.OutcomingAttack, ValueUnits.None }, 
            {Buff_Type.Health, ValueUnits.None }, 
            {Buff_Type.Evasion, ValueUnits.Percents }, 
            {Buff_Type.IncomingAccuracy, ValueUnits.Percents }, 
            {Buff_Type.OutcomingAccuracy, ValueUnits.Percents }, 
            {Buff_Type.IncomingDamageIncrease, ValueUnits.None }, 
            {Buff_Type.Movement, ValueUnits.Cells }, 
            {Buff_Type.AdditionalArmor, ValueUnits.None }, 
        };

        public static ValueUnits GetBuffUnits(Buff_Type buffType) => _buffUnits[buffType];
    }

    public static class EffectsDisplayHelpers
    {
        public static Dictionary<string, string> GetDisplayableParameters(this ITickEffect effect)
        {
            var result = new Dictionary<string, string>();
            var tickEffect = (TickEffectBase)effect;
            if (effect is StatsBuffEffect statsBuffEffect)
            {
                int GetModifiedValue(int value, int scaleValue) 
                    => Mathf.RoundToInt(statsBuffEffect.IsMultiplier
                        ? scaleValue * (1 + statsBuffEffect.Modifier)
                        : value + statsBuffEffect.Modifier);

                var buffName = Localization.Current.GetBuffName(statsBuffEffect.StatType);
                string value;

                if (statsBuffEffect.ScaleFromWhom == ScaleFromWhom.Target)
                {
                    value = statsBuffEffect.IsMultiplier
                    ? $"{(statsBuffEffect.Modifier > 1 ? "+" : "")}{statsBuffEffect.Modifier * 100}%"
                    : $"{(statsBuffEffect.Modifier >= 0 ? "+" : "")}{statsBuffEffect.Modifier}";
                }
                else if (statsBuffEffect.ScaleFromWhom == ScaleFromWhom.Caster)
                {
                    var statValue = statsBuffEffect.StatType switch
                    {
                        Buff_Type.Accuracy => statsBuffEffect.Caster.Stats.Accuracy,
                        Buff_Type.Attack => statsBuffEffect.Caster.Stats.Attack,
                        Buff_Type.Evasion => statsBuffEffect.Caster.Stats.Evasion,
                        Buff_Type.Health => statsBuffEffect.Caster.Stats.Health,
                        Buff_Type.Movement => statsBuffEffect.Caster.Stats.Movement,
                        Buff_Type.AdditionalArmor => statsBuffEffect.Caster.Stats.UnmodifiedArmor,
                        _ => throw new NotImplementedException()
                    };
                    var modValue = GetModifiedValue(statValue, statValue);
                    value = $"{(modValue > 0 ? "+" : "")}{modValue}";
                }
                else
                    throw new NotImplementedException();
                result.AddDisplayedParameter(buffName, value);
            }
            else if (effect is DamageOverTimeEffect damageOverTimeEffect)
            {
                var parameterName = damageOverTimeEffect.DamageHealTarget switch
                {
                    DamageHealTarget.Normal => "Урон",
                    DamageHealTarget.OnlyArmor => "Урон броне",
                    DamageHealTarget.OnlyHealth => "Урон ОЗ",
                    _ => throw new NotImplementedException()
                };
                result.AddDisplayedParameter(parameterName, damageOverTimeEffect.Damage);
            }
            else if (effect is IncomingBuff incomingBuff)
            {
                switch (incomingBuff.IncomingBuffType)
                {
                    case Buff_Type.IncomingAccuracy:
                        var sign = incomingBuff.Modificator >= 0 ? "+" : "";
                        result.AddDisplayedParameter("Вход. точность", $"{sign}{ incomingBuff.Modificator }");
                        break;
                    case Buff_Type.IncomingDamageIncrease:
                        result.AddDisplayedParameter("Урон взрывом", $"+{ (incomingBuff.Modificator - 1) * 100 }%");
                        break;
                    case Buff_Type.IncomingDamageReduction:
                        var dmgReduction = Mathf.RoundToInt((1 - (1 / incomingBuff.Modificator)) * 100);
                        result.AddDisplayedParameter("Урон взрывом", $"-{ dmgReduction }%");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
                throw new NotImplementedException();
            result.AddDisplayedParameter("Длительность", tickEffect.Duration);
            return result;
        }





        public static Dictionary<string, string> GetDisplayableParameters(
            this AbilityEffect effect, IReadOnlyBattleStats casterStats)
        {
            var result = new Dictionary<string, string>();
            if (effect.HasProbability) result.AddDisplayedParameter("Шанс", effect.Probability.ToString(), ValueUnits.Percents);
            if (effect.Type == AbilityEffectType.Damage || effect.Type == AbilityEffectType.Heal)
            {
                int damageHealSize;
                string valueEnding = effect.Amounts > 1 ? $" x {effect.Amounts}" : "";
                switch (effect.ScaleFrom)
                {
                    case AbilityScaleFrom.Attack:
                        damageHealSize = casterStats.Attack;
                        break;
                    case AbilityScaleFrom.Health:
                        damageHealSize = casterStats.UnmodifiedHealth;
                        break;
                    case AbilityScaleFrom.Movement:
                        damageHealSize = casterStats.UnmodifiedMovement;
                        break;
                    case AbilityScaleFrom.Distance:
                        damageHealSize = casterStats.Attack;
                        valueEnding = $" /{Localization.Current.GetUnits(ValueUnits.Cells)}";
                        break;
                    default:
                        throw new ArgumentException();
                }
                var displayedValue = $"{damageHealSize * effect.Scale}{valueEnding}";
                if (effect.Type == AbilityEffectType.Damage)
                    result.AddDisplayedParameter("Урон", displayedValue);
                if (effect.Type == AbilityEffectType.Heal)
                    result.AddDisplayedParameter("Лечение", displayedValue);
            }
            else if (effect.Type == AbilityEffectType.Modificator && effect.Modificator == ModificatorType.Accuracy)
            {
                var valuePrefix = effect.ModificatorValue > 0 ? "+" : "";
                result.AddDisplayedParameter("Точность", $"{valuePrefix}{effect.ModificatorValue}", ValueUnits.Percents);
            }
            else if (effect.Type == AbilityEffectType.TickingBuff || effect.Type == AbilityEffectType.OverTime)
            {
                if (effect.Type == AbilityEffectType.TickingBuff)
                {
                    var valuePrefix = effect.BuffModificator > 0 ? "+" : "";
                    result.AddDisplayedParameter(
                        Localization.Current.GetBuffName(effect.BuffType),
                        $"{valuePrefix}{effect.BuffModificator}",
                        EffectView.GetBuffUnits(effect.BuffType));
                }
                if (effect.Type == AbilityEffectType.OverTime)
                {
                    result.AddDisplayedParameter(Localization.Current.GetOvertimeTypeName(effect.OverTimeType), effect.TickValue);
                }
                result.AddDisplayedParameter("Длительность", effect.Duration);
            }
            else if (effect.Type == AbilityEffectType.Move)
            {
                result.AddDisplayedParameter("Дальность", casterStats.UnmodifiedMovement);
            }
            else if (effect.Type != AbilityEffectType.Modificator)
            {
                var message = $"EffectType {effect.Type} cannot be displayed since it's parameters haven't been described.";
                Debug.LogError(message);
                //throw new NotImplementedException(message);
            }
            return result;
        }

        public static void AddDisplayedParameter(
            this Dictionary<string, string> parameters, string name, string value, ValueUnits units = ValueUnits.None)
        {
            parameters.Add($"{name}: ", $"{value}{Localization.Current.GetUnits(units)}");
        }

        public static void AddDisplayedParameter(
            this Dictionary<string, string> parameters, string name, int value, ValueUnits units = ValueUnits.None)
            => parameters.AddDisplayedParameter(name, value.ToString(), units);

        public static void AddDisplayedParameter(
            this Dictionary<string, string> parameters, string name, float value, ValueUnits units = ValueUnits.None)
            => parameters.AddDisplayedParameter(name, value.ToString(), units);
    }
}
