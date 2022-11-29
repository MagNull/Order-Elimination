using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using System.Linq;

namespace UIManagement
{
    public class AbilityDescriptionPanel : UIPanel
    {
        public override PanelType PanelType => PanelType.AbilityDescription;
        [SerializeField] private Image _abilityIcon;
        [SerializeField] private TextMeshProUGUI _abilityName;
        [SerializeField] private TextMeshProUGUI _abilityDescription;
        [SerializeField] private RectTransform _effectsHolder;
        [SerializeField] private EffectDescriptionWindow _effectDescriptionPrefab;
        [SerializeReference] private List<EffectDescriptionWindow> Effects;

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void UpdateAbilityDescription(AbilityInfo abilityInfo)
        {
            _abilityName.text = abilityInfo.Name;
            _abilityDescription.text = abilityInfo.Description;
            _abilityIcon.sprite = abilityInfo.Icon;
            Effects = new List<EffectDescriptionWindow>();
            foreach (var effect in abilityInfo.TargetEffects.Concat(abilityInfo.AreaEffects))
            {
                var newEffectWindow = Instantiate(_effectDescriptionPrefab, _effectsHolder);
                newEffectWindow.UpdateEffectDescription(effect);
                Effects.Add(newEffectWindow);
            }
        }
    }
}
