using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using OrderElimination.AbilitySystem;
using System.Linq;

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
        private List<EffectDescriptionWindow> _effects = new();

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void UpdateAbilityData(
            IActiveAbilityData abilityData, ValueCalculationContext calculationContext)
        {
            _abilityParameters.Clear();
            var view = abilityData.View;
            _abilityName.text = view.Name;
            _abilityIcon.sprite = view.Icon;
            _abilityDescription.text = view.Description;
            RemoveEffects();
            Debug.Log($"Effects: {abilityData.GameRepresentation.EffectRepresentations.Count}");
            foreach (var e in abilityData.GameRepresentation.EffectRepresentations
                .Where(e => !e.EffectData.View.IsHidden))
            {
                var newEffectWindow = Instantiate(_effectDescriptionPrefab, _effectsHolder);
                newEffectWindow.UpdateEffectDescription(e.EffectData, calculationContext);
                if (e.ApplyChance.CanBePrecalculatedWith(calculationContext))
                    newEffectWindow.AddProbability(e.ApplyChance.GetValue(calculationContext));
                else
                    newEffectWindow.AddProbability(e.ApplyChance.GetSimplifiedFormula(calculationContext));
                _effects.Add(newEffectWindow);
            }
        }

        public void RemoveEffects()
        {
            var elementsToRemove = _effects.ToArray();
            _effects.Clear();
            foreach (var e in elementsToRemove)
            {
                Destroy(e.gameObject);
            }
        }
    }
}