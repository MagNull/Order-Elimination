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

    [Serializable]
    public class EffectView
    {
        public bool DisplayAsMainEffect;
        [ShowIf("@!DisplayAsMainEffect")]
        public string EffectName;
        [ShowIf("@!DisplayAsMainEffect")]
        public Sprite EffectIcon;

        public EffectView(Sprite icon, string name)
        {
            EffectIcon = icon;
            EffectName = name;
        }

        private static readonly Dictionary<BuffType, ValueUnits> _buffUnits = new()
        {
            {BuffType.Attack, ValueUnits.None }, 
            {BuffType.Health, ValueUnits.None }, 
            {BuffType.Evasion, ValueUnits.Percents }, 
            {BuffType.IncomingAccuracy, ValueUnits.Percents }, 
            {BuffType.IncomingAttack, ValueUnits.None }, 
            {BuffType.Movement, ValueUnits.Cells }, 
        };

        public static ValueUnits GetBuffUnits(BuffType buffType) => _buffUnits[buffType];
    }

    public static class EffectsDisplayHelpers
    {
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
                        valueEnding = $"/{Localization.Current.GetUnits(ValueUnits.Cells)}";
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
            if (effect.Type == AbilityEffectType.Modificator && effect.Modificator == ModificatorType.Accuracy)
            {
                var valuePrefix = effect.ModificatorValue > 0 ? "+" : "";
                result.AddDisplayedParameter("Точность", $"{valuePrefix}{effect.ModificatorValue}", ValueUnits.Percents);
            }
            if (effect.Type == AbilityEffectType.Buff || effect.Type == AbilityEffectType.OverTime)
            {
                if (effect.Type == AbilityEffectType.Buff)
                {
                    var valuePrefix = effect.BuffValue > 0 ? "+" : "";
                    result.AddDisplayedParameter(
                        Localization.Current.GetBuffName(effect.BuffType), 
                        $"{valuePrefix}{effect.BuffValue}", 
                        EffectView.GetBuffUnits(effect.BuffType));
                }
                if (effect.Type == AbilityEffectType.OverTime)
                {
                    result.AddDisplayedParameter(Localization.Current.GetOvertimeTypeName(effect.OverTimeType), effect.TickValue);
                }
                result.AddDisplayedParameter("Длительн", effect.Duration, ValueUnits.Turns);
            };
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
    }
}
