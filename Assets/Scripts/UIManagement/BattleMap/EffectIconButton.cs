using CharacterAbility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    public class EffectIconButton : MonoBehaviour
    {
        [SerializeField]
        private HoldableButton _button;
        [SerializeField]
        private Image _icon;
        public ITickEffectView CurrentEffect { get; private set; }
        public event Action<EffectIconButton> Clicked;

        public void UpdateEffectInfo(ITickEffectView effectView)
        {
            CurrentEffect = effectView;
            _icon.enabled = true;
            _icon.sprite = effectView.EffectIcon;
            _button.Clicked -= OnClicked;
            _button.Clicked += OnClicked;
        }

        public void RemoveEffectInfo()
        {
            _icon.enabled = true;
            _icon.sprite = null;
        }

        public void OnClicked(HoldableButton button)
        {
            Clicked?.Invoke(this);
            print($"Ёффект \"{CurrentEffect.EffectName}\"");
        }
    } 
}
