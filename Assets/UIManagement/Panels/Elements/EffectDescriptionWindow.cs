using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterAbility;

namespace UIManagement.Elements
{
    public class EffectDescriptionWindow : MonoBehaviour
    {
        [SerializeField] private Image _effectIcon;
        [SerializeField] private TextMeshProUGUI _effectName;
        [SerializeField] private IconTextValueList _parameters;

        public void UpdateEffectDescription(AbilityEffect effectInfo)
        {
            //_effectIcon = effectInfo.Icon;
            _effectName.text = effectInfo.DescriptionFlag;
            _parameters.Clear();
        }
    }
}
