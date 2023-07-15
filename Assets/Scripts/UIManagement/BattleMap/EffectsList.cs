using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;

namespace UIManagement.Elements
{
    public class EffectsList : MonoBehaviour
    {
        [SerializeField]
        private ClickableEffectButton _effectButtonPrefab;
        [ShowInInspector, SerializeField]
        private Dictionary<EffectCharacter, RectTransform> _effectsHolderLines = new();
        public float effectAppearTime;
        public Ease effectAppearEase = Ease.Flash;
        private Dictionary<IEffectData, ClickableEffectButton> _buttonsByEffects = new();
        private Dictionary<ClickableEffectButton, IEffectData> _effectsByButtons = new();
        private Dictionary<IEffectData, List<BattleEffect>> _effectsByData = new();

        public void UpdateEffects(IEnumerable<BattleEffect> effects)
        {
            //var currentEffectData = effects.GroupBy(e => e.EffectData).Select(g => g.Key).ToArray();
            //var effectsToRemove = _effectsByData.Keys.Except(currentEffectData).ToArray();
            //foreach (var d in effectsToRemove)
            //{

            //}
            //var displayableEffects = effects.Where(e => !e.EffectData.View.IsHidden).ToArray();
            //var effectsToAdd = new List<IEffectData>();
            //foreach (var effect in displayableEffects)
            //{
            //    if (!_buttonsByEffects.ContainsKey(effect.EffectData))
            //        effectsToAdd.Add(effect);
            //}
            //var effectsToRemove = _buttonsByEffects.Keys.Except(displayableEffects).ToArray();
            //foreach (var effect in effectsToRemove)
            //    RemoveEffect(effect);
            //foreach (var effect in effectsToAdd)
            //{
            //    var effectButton = Instantiate(_effectButtonPrefab, null);
            //    _buttonsByEffects.Add(effect, effectButton);
            //    _effectsByButtons.Add(effectButton, effect);
            //    effectButton.ButtonImage.sprite = effect.EffectData.View.Icon;
            //    effectButton.transform.localScale = Vector3.one * 0.1f;
            //    effectButton.transform.DOScale(1, effectAppearTime).SetEase(effectAppearEase);
            //    effectButton.Clicked -= OnEffectButtonClicked;
            //    effectButton.Clicked += OnEffectButtonClicked;
            //}
        }

        public void RemoveEffect(BattleEffect effect)
        {
            //var effectButton = _buttonsByEffects[effect];
            //effectButton.Clicked -= OnEffectButtonClicked;
            //_buttonsByEffects.Remove(effect);
            //_effectsByButtons.Remove(effectButton);

            //effectButton.DOComplete();
            //var disappearTime = effectButton.transform.localScale.magnitude * effectAppearTime / 2;
            //effectButton.transform
            //    .DOScale(0.1f, disappearTime)
            //    .SetEase(effectAppearEase)
            //    .OnComplete(() => Destroy(effectButton.gameObject));
        }

        public void ClearEffects()
        {
            //var buttonsToRemove = _buttonsByEffects.Values.ToArray();
            //_buttonsByEffects.Clear();
            //_effectsByButtons.Clear();
            //foreach (var b in buttonsToRemove)
            //{
            //    b.Clicked -= OnEffectButtonClicked;
            //    Destroy(b.gameObject);
            //}
        }

        private void OnEffectButtonClicked(HoldableButton effectButton)
        {
            //var effect = _effectsByButtons[effectButton];
            //var descrWindow = (EffectsDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.EffectsDesriptionList);
            //descrWindow.UpdateEffectsList(_buttonsByEffects.Keys.ToArray());
        }
    } 
}
