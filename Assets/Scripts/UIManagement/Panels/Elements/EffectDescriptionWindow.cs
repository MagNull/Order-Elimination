using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OrderElimination.AbilitySystem;

namespace UIManagement.Elements
{
    public class EffectDescriptionWindow : MonoBehaviour
    {
        [SerializeField] private Image _effectIcon;
        [SerializeField] private TextMeshProUGUI _effectName;
        [SerializeField] private IconTextValueList _parameters;

        public void UpdateEffectDescription(BattleEffect effect)
        {
            var view = effect.EffectData.View;
            _effectName.text = view.Name;
            _effectIcon.sprite = view.Icon;
            _parameters.Clear();
            throw new System.NotImplementedException();
        }
    }
}
