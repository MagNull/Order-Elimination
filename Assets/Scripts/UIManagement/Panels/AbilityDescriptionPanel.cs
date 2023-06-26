using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using OrderElimination.AbilitySystem;

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
            throw new System.NotImplementedException();
        }
    }
}