using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;

namespace UIManagement.Elements
{
    public class EffectsList : MonoBehaviour
    {
        [SerializeField]
        private EffectIconButton _effectPrefab;
        [SerializeField]
        private RectTransform _effectsHolder;
        public float effectAppearTime;
        public Ease effectAppearEase = Ease.Flash;
        public event Action<EffectIconButton> EffectButtonClicked;
        private Dictionary<ITickEffect, EffectIconButton> _effects = new Dictionary<ITickEffect, EffectIconButton>();

        public void UpdateEffects(ITickEffect[] effects)
        {
            var displayableEffects = effects.Where(e => e.GetEffectView().DisplayWhenApplied).ToArray();
            var effectsToAdd = new List<ITickEffect>();
            foreach (var e in displayableEffects)
            {
                if (!_effects.ContainsKey(e))
                    effectsToAdd.Add(e);
            }
            var effectsToRemove = _effects.Keys.Except(displayableEffects).ToArray();
            foreach (var e in effectsToRemove)
                RemoveEffect(e);
            foreach (var e in effectsToAdd)
            {
                var effectIcon = Instantiate(_effectPrefab, _effectsHolder);
                _effects.Add(e, effectIcon);
                effectIcon.UpdateEffectInfo(e.GetEffectView());
                effectIcon.transform.localScale = Vector3.one * 0.1f;
                effectIcon.transform.DOScale(1, effectAppearTime).SetEase(effectAppearEase);
                effectIcon.Clicked += OnEffectButtonClicked;
            }
        }

        public void RemoveEffect(ITickEffect effect)
        {
            var effectToRemove = _effects[effect];
            effectToRemove.Clicked -= OnEffectButtonClicked;
            _effects.Remove(effect);
            Destroy(effectToRemove.gameObject);
        }

        public void ClearEffects()
        {
            var effectsToRemove = _effects.Values.ToArray();
            _effects.Clear();
            foreach (var e in effectsToRemove)
            {
                e.Clicked -= OnEffectButtonClicked;
                Destroy(e.gameObject);
            }
        }

        public void OnEffectButtonClicked(EffectIconButton effectButton)
        {
            EffectButtonClicked?.Invoke(effectButton);
            var descrWindow = (EffectsDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.EffectsDesriptionList);
            descrWindow.UpdateEffectsList(_effects.Keys.ToArray());
        }
    } 
}
