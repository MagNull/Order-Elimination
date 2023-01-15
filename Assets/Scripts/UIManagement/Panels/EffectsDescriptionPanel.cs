using CharacterAbility;
using OrderElimination;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    public class EffectsDescriptionPanel : UIPanel
    {
        public override PanelType PanelType => PanelType.EffectsDesriptionList;
        [SerializeField]
        private EffectDescriptionWindow _effectDescriptionPrefab;
        [SerializeField]
        private RectTransform _effectsHolder;
        [SerializeField]
        private RectTransform _scalableWindowBase;
        private List<EffectDescriptionWindow> _effects = new List<EffectDescriptionWindow>();


        public void UpdateEffectsList(ITickEffect[] effects)
        {
            RemoveEffects();
            foreach (var e in effects)
            {
                var newEffectWindow = Instantiate(_effectDescriptionPrefab, _effectsHolder);
                newEffectWindow.UpdateEffectDescription(e);
                _effects.Add(newEffectWindow);
            }
            //var rect = _scalableWindowBase.rect;
            //var spriteRect = _scalableWindowBase.GetComponent<Image>().sprite.rect;
            //var perElementSizeIncrement = _effectDescriptionPrefab.GetComponent<RectTransform>().rect.height;
            //_scalableWindowBase.rect.Set(
            //    rect.x, rect.y, spriteRect.width, spriteRect.height + perElementSizeIncrement * (effects.Length - 1));
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
