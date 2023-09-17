using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.UI;
using OrderElimination.Infrastructure;

namespace UIManagement.Elements
{
    public class EffectDescriptionWindow : MonoBehaviour
    {
        [SerializeField] private Image _effectIcon;
        [SerializeField] private TextMeshProUGUI _effectName;
        [SerializeField] private IconTextValueList _parameters;

        private IEffectData _effectData;

        public void UpdateEffectDescription(BattleEffect effect)
        {
            var calculationContext = ValueCalculationContext.Full(
                effect.BattleContext,
                null,
                effect.EffectApplier,
                effect.EffectHolder);
            UpdateEffectDescription(effect.EffectData, calculationContext);
            if (effect.LeftDuration.HasValue)
                _parameters.Add(null, "Длительность", effect.LeftDuration.Value);
        }

        public void UpdateEffectDescription(IEffectData effectData, 
            ValueCalculationContext calculationContext)
        {
            _effectData = effectData;
            var view = effectData.View;
            _effectName.text = view.Name;
            _effectIcon.sprite = view.Icon;
            _parameters.Clear();
            var humanRepresentation = EffectHumanRepresentation.FromEffect(effectData, calculationContext);
            foreach (var parameter in humanRepresentation.Parameters)
            {
                _parameters.Add(null, parameter.Name, parameter.Value, parameter.ValueUnits);
            }
        }

        public bool AddApplyingDuration()
        {
            if (_effectData == null)
                throw new System.InvalidOperationException();
            if (_effectData.TemporaryEffectFunctionaity != null)
            {
                var duration = _effectData.TemporaryEffectFunctionaity.ApplyingDuration;
                _parameters.Add(null, "Длительность", duration, ValueUnits.None);
                return true;
            }
            return false;
        }

        public void AddProbability(float probability)
        {
            _parameters.Add(null, "Шанс", probability * 100, ValueUnits.Percents);
        }

        public void AddProbability(string probability)
        {
            _parameters.Add(null, "Шанс", probability);
        }
    }
}
