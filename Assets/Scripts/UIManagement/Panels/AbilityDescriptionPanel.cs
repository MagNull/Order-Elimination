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
        [SerializeReference]
        private List<EffectDescriptionWindow> Effects;

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void UpdateAbilityData(IActiveAbilityData abilityData)
        {
            _abilityParameters.Clear();
            _effectsHolder.gameObject.SetActive(false);
            var view = abilityData.View;
            _abilityName.text = view.Name;
            _abilityIcon.sprite = view.Icon;
            _abilityDescription.text = view.Description;
        }
    }
}