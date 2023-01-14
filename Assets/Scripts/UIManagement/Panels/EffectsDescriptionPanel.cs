using CharacterAbility;
using OrderElimination;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIManagement.Elements
{
    public class EffectsDescriptionPanel : UIPanel
    {
        public override PanelType PanelType => PanelType.EffectsDesriptionList;
        [SerializeField]
        private EffectDescriptionWindow _effectDescriptionPrefab;
        [SerializeField]
        private RectTransform _effectsHolder;
        private List<EffectDescriptionWindow> _effects = new List<EffectDescriptionWindow>();


        public void UpdateEffectsList(ITickEffectView[] effectViews)
        {
            RemoveEffects();
            foreach (var e in effectViews)
            {
                var newEffectWindow = Instantiate(_effectDescriptionPrefab, _effectsHolder);
                newEffectWindow.UpdateEffectDescription(e, null);
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
