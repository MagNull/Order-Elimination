using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using OrderElimination.Infrastructure;

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
        private Dictionary<IEffectData, BattleEffect[]> _effectsByData = new();

        public void UpdateEffects(IEnumerable<BattleEffect> currentEffects)
        {
            var effects = currentEffects.Where(e => !e.EffectData.View.IsHidden).ToArray();
            var currentEffectDatas = effects.GroupBy(e => e.EffectData).Select(g => g.Key).ToArray();
            var effectsToRemove = _effectsByData.Keys.Except(currentEffectDatas).ToArray();
            foreach (var effectData in effectsToRemove)
            {
                RemoveEffect(effectData);
            }
            foreach (var effectData in currentEffectDatas)
            {
                if (!_effectsByData.ContainsKey(effectData))
                    _effectsByData.Add(effectData, new BattleEffect[0]);
                _effectsByData[effectData] = effects.Where(e => e.EffectData == effectData).ToArray();
                var lineParent = _effectsHolderLines[effectData.EffectCharacter];
                ClickableEffectButton button;
                if (!_buttonsByEffects.ContainsKey(effectData))
                {
                    button = Instantiate(_effectButtonPrefab, lineParent);
                    button.IconImage.sprite = effectData.View.Icon;
                    button.transform.localScale = Vector3.one * 0.1f;
                    button.transform.DOScale(1, effectAppearTime).SetEase(effectAppearEase);
                    button.Clicked -= OnEffectButtonClicked;
                    button.Clicked += OnEffectButtonClicked;
                    _buttonsByEffects.Add(effectData, button);
                    _effectsByButtons.Add(button, effectData);
                }
                button = _buttonsByEffects[effectData];
                var stackCount = _effectsByData[effectData].Length;
                if (stackCount > 1)
                    button.StackNumbersText.text = $"x{stackCount}";
                else
                    button.StackNumbersText.text = "";
            }
            foreach (var line in _effectsHolderLines)
            {
                if (line.Value.childCount > 0)
                    line.Value.gameObject.SetActive(true);
                else
                    line.Value.gameObject.SetActive(false);
            }
        }

        public void RemoveEffect(IEffectData effect)
        {
            var effectButton = _buttonsByEffects[effect];
            effectButton.Clicked -= OnEffectButtonClicked;
            _buttonsByEffects.Remove(effect);
            _effectsByButtons.Remove(effectButton);
            _effectsByData.Remove(effect);

            effectButton.DOComplete();
            var disappearTime = effectButton.transform.localScale.magnitude * effectAppearTime / 2;
            effectButton.transform
                .DOScale(0.1f, disappearTime)
                .SetEase(effectAppearEase)
                .OnComplete(() => Destroy(effectButton.gameObject));
        }

        public void ClearEffects()
        {
            var buttonsToRemove = _buttonsByEffects.Values.ToArray();
            _buttonsByEffects.Clear();
            _effectsByButtons.Clear();
            _effectsByData.Clear();
            foreach (var b in buttonsToRemove)
            {
                b.Clicked -= OnEffectButtonClicked;
                Destroy(b.gameObject);
            }
        }

        private void OnEffectButtonClicked(ClickableEffectButton effectButton)
        {
            var effect = _effectsByButtons[effectButton];
            var descrWindow = (EffectsDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.EffectsDesriptionList);
            descrWindow.UpdateEffectsList(_effectsByData.SelectMany(kv => kv.Value));
        }
    } 
}
