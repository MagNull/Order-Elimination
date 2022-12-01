using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using System.Linq;
using Sirenix.OdinInspector;

namespace UIManagement
{
    public class AbilityDescriptionPanel : UIPanel
    {
        #region ToRemove
        [OnValueChanged(nameof(OnTestAbility))]
        public AbilityInfo TestingAbilityInfo;
        private void OnTestAbility() => UpdateAbilityDescription(TestingAbilityInfo);
        #endregion ToRemove
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

        public void UpdateAbilityDescription(AbilityInfo abilityInfo)
        {
            Debug.Log("Updating...");
            _abilityName.text = abilityInfo.Name;
            _abilityDescription.text = abilityInfo.Description;
            _abilityIcon.sprite = abilityInfo.Icon;
            _abilityParameters.Clear();

            if (HasEffect(abilityInfo, "Урон", out var effectWithFlag))
            {

            }
            //foreach (var p in abilityInfo.)
            //if (abilityInfo.Distance != null)
    
            foreach (var e in _effectsHolder.GetComponentsInChildren<EffectDescriptionWindow>())
                Destroy(e.gameObject);
            Effects = new List<EffectDescriptionWindow>();

            foreach (var effect in abilityInfo.TargetEffects.Concat(abilityInfo.AreaEffects))
            {
                var newEffectWindow = Instantiate(_effectDescriptionPrefab, _effectsHolder);
                newEffectWindow.UpdateEffectDescription(effect);
                Effects.Add(newEffectWindow);
            }
        }

        private bool HasEffect(AbilityInfo abilityInfo, string descriptionFlag, out AbilityEffect effect)
        {
            AbilityEffect targetEffect = abilityInfo.GetTargetEffectByFlag(descriptionFlag);
            AbilityEffect areaEffect = abilityInfo.GetAreaEffectByFlag(descriptionFlag);
            var hasTargetEffect = !targetEffect.Equals(default(AbilityEffect));
            var hasAreaEffect = !areaEffect.Equals(default(AbilityEffect));
            if (hasAreaEffect && hasTargetEffect)
                throw new System.ArgumentException("Multiple effects with the same description flag. Unable to resolve.");
            effect = hasTargetEffect ? targetEffect : hasAreaEffect ? areaEffect : default;
            return hasTargetEffect || hasAreaEffect;
        }
    }
}
