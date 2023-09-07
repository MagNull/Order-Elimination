using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.UI;

namespace UIManagement.Elements
{
    public class EffectDescriptionWindow : MonoBehaviour
    {
        [SerializeField] private Image _effectIcon;
        [SerializeField] private TextMeshProUGUI _effectName;
        [SerializeField] private IconTextValueList _parameters;

        public void UpdateEffectDescription(BattleEffect effect)
        {
            var data = effect.EffectData;
            var view = data.View;
            _effectName.text = view.Name;
            _effectIcon.sprite = view.Icon;
            _parameters.Clear();
            var humanRepresentation = EffectHumanRepresentation.FromEffect(effect.EffectData);
            foreach (var parameter in humanRepresentation.Parameters)
            {
                _parameters.Add(null, parameter.ParameterName, parameter.Value, parameter.ValueUnits);
            }
            if (effect.LeftDuration.HasValue)
                _parameters.Add(null, "Длительность:", effect.LeftDuration.Value);
        }
    }
}
