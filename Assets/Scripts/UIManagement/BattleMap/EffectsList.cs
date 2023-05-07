using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;
using OrderElimination.AbilitySystem;

namespace UIManagement.Elements
{
    public class EffectsList : MonoBehaviour
    {
        [SerializeField]
        private HoldableButton _effectButtonPrefab;
        [SerializeField]
        private RectTransform _effectsHolder;
        public float effectAppearTime;
        public Ease effectAppearEase = Ease.Flash;
        private Dictionary<BattleEffect, HoldableButton> _buttonsByEffects = new();
        private Dictionary<HoldableButton, BattleEffect> _effectsByButtons = new();

        public void UpdateEffects(IEnumerable<BattleEffect> effects)
        {
            var displayableEffects = effects.Where(e => !e.EffectData.View.IsHidden).ToArray();
            var effectsToAdd = new List<BattleEffect>();
            foreach (var effect in displayableEffects)
            {
                if (!_buttonsByEffects.ContainsKey(effect))
                    effectsToAdd.Add(effect);
            }
            var effectsToRemove = _buttonsByEffects.Keys.Except(displayableEffects).ToArray();
            foreach (var effect in effectsToRemove)
                RemoveEffect(effect);
            foreach (var effect in effectsToAdd)
            {
                var effectButton = Instantiate(_effectButtonPrefab, _effectsHolder);
                _buttonsByEffects.Add(effect, effectButton);
                _effectsByButtons.Add(effectButton, effect);
                effectButton.ButtonImage.sprite = effect.EffectData.View.Icon;
                effectButton.transform.localScale = Vector3.one * 0.1f;
                effectButton.transform.DOScale(1, effectAppearTime).SetEase(effectAppearEase);
                effectButton.Clicked -= OnEffectButtonClicked;
                effectButton.Clicked += OnEffectButtonClicked;
            }
        }

        public void RemoveEffect(BattleEffect effect)
        {
            var effectButton = _buttonsByEffects[effect];
            effectButton.Clicked -= OnEffectButtonClicked;
            _buttonsByEffects.Remove(effect);
            _effectsByButtons.Remove(effectButton);
            Destroy(effectButton.gameObject);
        }

        public void ClearEffects()
        {
            var buttonsToRemove = _buttonsByEffects.Values.ToArray();
            _buttonsByEffects.Clear();
            _effectsByButtons.Clear();
            foreach (var b in buttonsToRemove)
            {
                b.Clicked -= OnEffectButtonClicked;
                Destroy(b.gameObject);
            }
        }

        private void OnEffectButtonClicked(HoldableButton effectButton)
        {
            var effect = _effectsByButtons[effectButton];
            var descrWindow = (EffectsDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.EffectsDesriptionList);
            //TODO Display Effects list
            //descrWindow.UpdateEffectsList(_buttonsByEffects.Keys.ToArray());
        }
    } 
}
