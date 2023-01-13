using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    /// <summary>
    /// Don't use onClick event or you die. 
    /// Весь этот класс – параша, как и юнитивский Button. 
    /// В следующий раз лучше будет изобрести велосипед или использовать уже готовый ассет.
    /// </summary>
    [Serializable]
    public sealed class HoldableButton : Button
    {
        private int MillisecondsToHoldError => Math.Min(10, MillisecondsToHold);
        private float? _pressTime;
        private bool isPressed = false;
        private bool isPointerOnButton = false;

        public event Action<HoldableButton> Clicked;
        public event Action<HoldableButton, float> Holded;
        [SerializeField]
        public int MillisecondsToHold = 700;
        [SerializeField]
        public Color ClickUnavalableTint = Color.red;

        [SerializeField]
        private bool _clickAvailable;
        public bool ClickAvailable
        {
            get => _clickAvailable;
            set
            {
                _clickAvailable = value;
                UpdateVisuals();
            }
        }
        [SerializeField]
        private bool _holdAvailable;
        public bool HoldAvailable
        {
            get => _holdAvailable;
            set
            {
                _holdAvailable = value;
                UpdateVisuals();
            }
        }

        [Obsolete("Deprecated. Use Clicked and Holded events instead.")]
        public ButtonClickedEvent onClick => onClick;

        public void UpdateVisuals()
        {
            targetGraphic.color = Color.white;
            if (!ClickAvailable)
                targetGraphic.color = ClickUnavalableTint;
        }

        protected override void OnEnable()
        {
            UpdateVisuals();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
                return;
            if (!ClickAvailable && !HoldAvailable)
                return;
            _pressTime = Time.unscaledTime;
            isPressed = true;
            base.OnPointerDown(eventData);
            UniTask.Create(WaitUntilHoldTime);
        }

        private async UniTask WaitUntilHoldTime()
        {
            if (!HoldAvailable) return;
            await UniTask.Delay(MillisecondsToHold, ignoreTimeScale: true);
            if (!interactable 
                || !isPressed
                || !isPointerOnButton)
                return;
            if (!HoldAvailable) return;
            var holdTime = Time.unscaledTime - _pressTime;
            if (holdTime * 1000 + MillisecondsToHoldError >= MillisecondsToHold)
            {
                base.OnPointerUp(new PointerEventData(EventSystem.current));
                OnHold(Mathf.Min(holdTime.Value, MillisecondsToHold / 1000f));
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable)
                return;
            base.OnPointerUp(eventData);
            if (!isPressed)
                return;
            isPressed = false;
            _pressTime = null;
            if (ClickAvailable && isPointerOnButton)
            {
                OnClick();
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!interactable)
                return;
            isPointerOnButton = false;
            _pressTime = null;
            base.OnPointerExit(eventData);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable)
                return;
            isPointerOnButton = true;
            if (isPressed)
            {
                _pressTime = Time.unscaledTime;
                UniTask.Create(WaitUntilHoldTime);
            }
            base.OnPointerEnter(eventData);
        }

        private void OnClick()
        {
            isPressed = false;
            Clicked?.Invoke(this);
        }

        private void OnHold(float holdTimeInSeconds)
        {
            isPressed = false;
            Holded?.Invoke(this, holdTimeInSeconds);
        }
    }
}