using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterAbility;
using UIManagement.trashToRemove_Mockups;
using OrderElimination;

namespace UIManagement.Elements
{
    public class EffectDescriptionWindow : MonoBehaviour
    {
        [SerializeField] private Image _effectIcon;
        [SerializeField] private TextMeshProUGUI _effectName;
        [SerializeField] private IconTextValueList _parameters;

        public void UpdateEffectDescription(AbilityEffect effectInfo, IReadOnlyBattleStats casterStats)
        {
            //_effectIcon = effectInfo.Icon;
            _effectName.text = effectInfo.Name;
            _parameters.Clear();
            foreach (var p in effectInfo.GetDisplayableParameters(casterStats))
            {
                _parameters.Add(null, p.Key, p.Value);
            }
        }
    }

    public static class EffectsDisplayHelpers
    {
        public static Dictionary<string, string> GetDisplayableParameters(
            this AbilityEffect effect, IReadOnlyBattleStats casterStats)
        {
            var result = new Dictionary<string, string>();
            if (effect.HasProbability) result.AddDisplayedParameter("Шанс", effect.Probability, ValueUnits.Percents);
            if (effect.Type == AbilityEffectType.Damage || effect.Type == AbilityEffectType.Heal)
            {
                var damageHealSize = effect.ScaleFrom == AbilityScaleFrom.Attack
                    ? casterStats.Attack
                    : effect.ScaleFrom == AbilityScaleFrom.Health
                        ? casterStats.UnmodifiedHealth 
                        : throw new System.ArgumentException();
                var multiplierEnding = effect.Amounts > 1 ? $" x {effect.Amounts}" : "";
                var displayedValue = $"{damageHealSize * effect.Scale}{multiplierEnding}";
                if (effect.Type == AbilityEffectType.Damage)
                    result.AddDisplayedParameter("Урон", displayedValue);
                if (effect.Type == AbilityEffectType.Heal)
                    result.AddDisplayedParameter("Лечение", displayedValue);
            }
            if (effect.Type == AbilityEffectType.Modificator && effect.Modificator == ModificatorType.Accuracy)
            {
                result.AddDisplayedParameter("Точность", $"{effect.ModificatorValue}", ValueUnits.Percents);
            }
            if (effect.Type == AbilityEffectType.Buff || effect.Type == AbilityEffectType.OverTime)
            {
                if (effect.Type == AbilityEffectType.Buff)
                {
                    result.AddDisplayedParameter(Localization.Current.GetBuffName(effect.BuffType), effect.BuffValue);
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
            parameters.Add($"{name}: ", $"{value}{Localization.Current.GetUnitName(units)}");
        }

        public static void AddDisplayedParameter(
            this Dictionary<string, string> parameters,string name, float value, ValueUnits units = ValueUnits.None)
            => parameters.AddDisplayedParameter(name, value.ToString(), units);

        public static void AddDisplayedParameter(
            this Dictionary<string, string> parameters, string name, int value, ValueUnits units = ValueUnits.None) 
            => parameters.AddDisplayedParameter(name, value.ToString(), units);
    }
}
