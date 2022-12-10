using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using System.Linq;
using Sirenix.OdinInspector;
using UIManagement.trashToRemove_Mockups;

namespace UIManagement
{
    public class AbilityDescriptionPanel : UIPanel
    {
        public override PanelType PanelType => PanelType.AbilityDescription;
        [SerializeField] private TextMeshProUGUI _abilityName;
        [SerializeField] private Image _abilityIcon;
        [SerializeField] private IconTextValueList _abilityParameters;
        [SerializeField] private TextMeshProUGUI _abilityDescription;
        [SerializeField] private EffectDescriptionWindow _effectDescriptionPrefab;
        [SerializeField] private RectTransform _effectsHolder;
        [SerializeReference] private List<EffectDescriptionWindow> Effects;

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void UpdateAbilityDescription(AbilityView abilityView)
        {
            var abilityInfo = abilityView.AbilityInfo;
            var casterStats = abilityView.Caster.Stats;

            _abilityName.text = abilityInfo.Name;
            _abilityDescription.text = abilityInfo.Description;
            _abilityIcon.sprite = abilityInfo.Icon;
            _abilityParameters.Clear();
    
            foreach (var e in _effectsHolder.GetComponentsInChildren<EffectDescriptionWindow>())
                Destroy(e.gameObject);
            Effects = new List<EffectDescriptionWindow>();

            foreach (var effect in abilityInfo.TargetEffects.Concat(abilityInfo.AreaEffects).Where(e => e.ShowInAbilityDescription))
            {
                if (effect.EffectView.DisplayAsMainEffect)
                {
                    foreach (var mainP in effect.GetDisplayableParameters(casterStats))
                    {
                        _abilityParameters.Add(null, mainP.Key, mainP.Value);
                    }
                    continue;
                }
                var newEffectWindow = Instantiate(_effectDescriptionPrefab, _effectsHolder);
                newEffectWindow.UpdateEffectDescription(effect, casterStats);
                Effects.Add(newEffectWindow);
            }
            if (abilityInfo.)
                _abilityParameters.Add(null, "Радиус", abilityInfo.AreaRadius.ToString(), ValueUnits.Cells);
            _abilityParameters.Add(null, "Откат: ", abilityInfo.CoolDown.ToString(), ValueUnits.Turns);
        }

        //private bool HasEffect(AbilityInfo abilityInfo, string descriptionFlag, out AbilityEffect effect)
        //{
        //    AbilityEffect targetEffect = abilityInfo.GetTargetEffectByFlag(descriptionFlag);
        //    AbilityEffect areaEffect = abilityInfo.GetAreaEffectByFlag(descriptionFlag);
        //    var hasTargetEffect = !targetEffect.Equals(default(AbilityEffect));
        //    var hasAreaEffect = !areaEffect.Equals(default(AbilityEffect));
        //    if (hasAreaEffect && hasTargetEffect)
        //        throw new System.ArgumentException("Multiple effects with the same description flag. Unable to resolve.");
        //    effect = hasTargetEffect ? targetEffect : hasAreaEffect ? areaEffect : default;
        //    return hasTargetEffect || hasAreaEffect;
        //}
    }
}
