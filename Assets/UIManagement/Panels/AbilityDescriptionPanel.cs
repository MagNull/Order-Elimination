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
        [SerializeReference]
        private List<EffectDescriptionWindow> Effects;

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

            foreach (var effect in abilityInfo.ActiveParams.TargetEffects.Concat(abilityInfo.ActiveParams.AreaEffects)
                         .Where(e => e.ShowInAbilityDescription))
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

            if (abilityInfo.ActiveParams.HasTarget && !abilityInfo.ActiveParams.DistanceFromMovement &&
                abilityInfo.ActiveParams.Distance != 0)
                _abilityParameters.Add(null, "Дальность: ", abilityInfo.ActiveParams.Distance.ToString(),
                    ValueUnits.Cells);
            if (abilityInfo.ActiveParams.HasAreaEffect)
                _abilityParameters.Add(null, "Радиус: ", abilityInfo.ActiveParams.AreaRadius.ToString(),
                    ValueUnits.Cells);
            _abilityParameters.Add(null, "Откат: ", abilityInfo.CoolDown.ToString(), ValueUnits.Turns);
        }
    }
}