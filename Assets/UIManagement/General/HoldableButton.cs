using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CharacterAbility
{
    // Сериализуемые поля назначать в инспекторе в Debug режиме!!
    //TODO выделить отдельный класс HoldableButton
    public class HoldableButton : Button
    {
        public event Action<HoldableButton> Clicked;
        public event Action<HoldableButton> Holded;
        [SerializeField]
        public int MillisecondsToHold = 700;
        public bool ClickAvailable;
        public bool HoldAvailable;
        private int MillisecondsToHoldError => Math.Min(10, MillisecondsToHold);
        private bool isPressed;
        private float? _pressedTime;
        private float? _releasedTime;
        private bool isPointerOnButton = false;

        private float? HoldingTimeInSeconds
        {
            get
            {
                if (!_pressedTime.HasValue || !_releasedTime.HasValue)
                    return null;
                if (_releasedTime < _pressedTime)
                    throw new Exception();
                return _releasedTime - _pressedTime.Value;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable || !isPointerOnButton)
                return;
            
            _pressedTime = Time.unscaledTime;
            _releasedTime = null;
            base.OnPointerDown(eventData);
            UniTask.Create(WaitUntilHoldTime);
            Debug.Log("Pressed");
        }

        protected async UniTask WaitUntilHoldTime()
        {
            await UniTask.Delay(MillisecondsToHold, ignoreTimeScale: true);
            if (!isPointerOnButton)
            {
                return;
            }
            _releasedTime = Time.unscaledTime;
            if (!HoldingTimeInSeconds.HasValue
                || HoldingTimeInSeconds.Value + MillisecondsToHoldError / 1000f < MillisecondsToHold / 1000f)
                return;
            OnHold();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable)
                return;
            _pressedTime = null;
            base.OnPointerUp(eventData);
            if (_releasedTime != null)
                return;
            OnClick();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            isPointerOnButton = false;
            _pressedTime = null;
            base.OnPointerExit(eventData);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            isPointerOnButton = true;
            _pressedTime = Time.unscaledTime;
            base.OnPointerExit(eventData);
        }

        private void OnClick()
        {
            _releasedTime = Time.unscaledTime;
            Clicked?.Invoke(this);
            Debug.Log("Clicked");
        }

        private void OnHold()
        {
            Debug.Log("Holded for " + HoldingTimeInSeconds.Value);
            Holded?.Invoke(this);
            OnPointerUp(new PointerEventData(EventSystem.current));
        }
    }
}