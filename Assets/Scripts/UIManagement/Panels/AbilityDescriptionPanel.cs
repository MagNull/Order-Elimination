using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using OrderElimination.AbilitySystem;
using System.Linq;
using Unity.VisualScripting;
using OrderElimination.Infrastructure;
using OrderElimination.AbilitySystem.UI;

namespace UIManagement
{
    public class AbilityDescriptionPanel : UIPanel
    {
        public override PanelType PanelType => PanelType.AbilityDescription;
        [SerializeField]
        private TextMeshProUGUI _abilityName;
        [SerializeField]
        private Image _abilityIcon;
        [SerializeField]
        private IconTextValueList _abilityParameters;
        [SerializeField]
        private TextMeshProUGUI _abilityDescription;
        [SerializeField]
        private EffectDescriptionWindow _effectDescriptionPrefab;
        [SerializeField]
        private RectTransform _effectsHolder;
        private List<EffectDescriptionWindow> _effects = new();

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void UpdateAbilityData(
            IActiveAbilityData abilityData, ValueCalculationContext calculationContext)
        {
            var view = abilityData.View;
            _abilityName.text = view.Name;
            _abilityIcon.sprite = view.Icon;
            _abilityDescription.text = view.Description;

            var parameters = new List<HumanValue>();

            foreach (var p in abilityData.View.CustomParameters)
            {
                var value = p.Value.GetSimplifiedFormula(calculationContext);
                parameters.Add(new HumanValue(p.Key, value));
            }

            foreach (var d in abilityData.GameRepresentation.DamageRepresentations)
            {
                var size = d.UnprocessedDamageSize.GetSimplifiedFormula(calculationContext);
                parameters.Add(
                    new("Урон", $"{size} x {d.TotalRepetitions}"));
                if (d.UnprocessedDamageAction.ArmorMultiplier != 1)
                    parameters.Add(
                        new("Урон броне", d.UnprocessedDamageAction.ArmorMultiplier, ValueUnits.Multiplier));
                if (d.UnprocessedDamageAction.HealthMultiplier != 1)
                    parameters.Add(
                        new("Урон здоровью", d.UnprocessedDamageAction.HealthMultiplier, ValueUnits.Multiplier));
                var accuracy = d.UnprocessedAccuracySize.GetInPercentOrSimplify(calculationContext);
                parameters.Add(new("Точность", accuracy));
            }

            foreach (var h in abilityData.GameRepresentation.HealRepresentations)
            {
                var size = h.UnprocessedHealSize.GetSimplifiedFormula(calculationContext);
                var healName = h.UnprocessedHealAction.HealPriority switch
                {
                    LifeStatPriority.ArmorOnly => "Броня",
                    LifeStatPriority.HealthOnly => "Здоровье",
                    _ => "Восстановление"
                };
                parameters.Add(new(healName, $"{size} x {h.TotalRepetitions}"));
                if (h.UnprocessedHealAction.ArmorMultiplier != 1)
                    parameters.Add(
                        new("Броне", h.UnprocessedHealAction.ArmorMultiplier, ValueUnits.Multiplier));
                if (h.UnprocessedHealAction.HealthMultiplier != 1)
                    parameters.Add(
                        new("Здоровью", h.UnprocessedHealAction.HealthMultiplier, ValueUnits.Multiplier));
            }

            _abilityParameters.Clear();
            foreach (var parameter in parameters.Distinct())
                //.Where(p => p.Value.Length < 20))
            {
                _abilityParameters.Add(null, parameter.Name, parameter.Value, parameter.ValueUnits);
            }

            RemoveEffects();
            foreach (var e in abilityData.GameRepresentation.EffectRepresentations
                .Where(e => !e.EffectData.View.IsHidden)
                .DistinctBy(e => e.EffectData))//Can skip same effects with different apply chance
            {
                var newEffectWindow = Instantiate(_effectDescriptionPrefab, _effectsHolder);
                newEffectWindow.UpdateEffectDescription(e.EffectData, calculationContext);
                newEffectWindow.AddApplyingDuration();
                if (e.ApplyChance.CanBePrecalculatedWith(calculationContext))
                {
                    var chance = e.ApplyChance.GetValue(calculationContext);
                    if (chance < 1)
                        newEffectWindow.AddProbability(e.ApplyChance.GetValue(calculationContext));
                }
                else
                    newEffectWindow.AddProbability(e.ApplyChance.GetSimplifiedFormula(calculationContext));
                _effects.Add(newEffectWindow);
            }
        }

        public void RemoveEffects()
        {
            var elementsToRemove = _effects.ToArray();
            _effects.Clear();
            foreach (var e in elementsToRemove)
            {
                Destroy(e.gameObject);
            }
        }
    }
}