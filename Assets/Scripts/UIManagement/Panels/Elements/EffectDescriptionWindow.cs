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
            _effectName.text = effectInfo.EffectView.EffectName;
            _effectIcon.sprite = effectInfo.EffectView.EffectIcon;
            _parameters.Clear();
            foreach (var p in effectInfo.GetDisplayableParameters(casterStats))
            {
                _parameters.Add(null, p.Key, p.Value); //параметрам эффекта не нужны иконки
            }
        }

        public void UpdateEffectDescription(ITickEffect effect)
        {
            var effectview = effect.GetEffectView();
            _effectName.text = effectview.EffectName;
            _effectIcon.sprite = effectview.EffectIcon;
            _parameters.Clear();
            foreach (var p in effect.GetDisplayableParameters())
            {
                _parameters.Add(null, p.Key, p.Value);
            }
        }
    }
}
