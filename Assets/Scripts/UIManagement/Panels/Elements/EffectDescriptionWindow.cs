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

        public void UpdateEffectDescription(BattleEffect effect)
        {
            UpdateEffectDescription(effect.EffectData);
            if (effect.LeftDuration.HasValue)
                _parameters.Add(null, "Длительность", effect.LeftDuration.Value);
        }

        public void UpdateEffectDescription(IEffectData effectData)
        {
            var view = effectData.View;
            _effectName.text = view.Name;
            _effectIcon.sprite = view.Icon;
            _parameters.Clear();
            var humanRepresentation = EffectHumanRepresentation.FromEffect(effectData);
            foreach (var parameter in humanRepresentation.Parameters)
            {
                _parameters.Add(null, parameter.ParameterName, parameter.Value, parameter.ValueUnits);
            }
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
