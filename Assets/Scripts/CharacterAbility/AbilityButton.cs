using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CharacterAbility
{
    // Сериализуемые поля назначать в инспекторе в Debug режиме!!
    //TODO выделить отдельный класс HoldableButton
    public class AbilityButton : Button
    {
        public event Action Casted;
        private AbilityView _abilityView;

        public event Action<AbilityButton> Clicked;
        [SerializeField]
        private Image _abilityImage;
        [SerializeField]
        private TextMeshProUGUI _abilityName;
        private static readonly int millisecondsToHold = 1000;
        private static readonly int errorHoldTimeInMilliseconds = Math.Min(10, millisecondsToHold);
        private float? _pressedTime;
        public float? HoldingTimeInSeconds
        {
            get
            {
                if (_pressedTime == null)
                    return null;
                if (Time.time < _pressedTime)
                    throw new Exception();
                return Time.time - _pressedTime.Value;
            }
        }

        private void Awake()
        {
            interactable = false;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
                return;
            _pressedTime = Time.time;
            base.OnPointerDown(eventData);
            UniTask.Create(WaitUntilHoldTime);
            Debug.Log("Pressed");
        }

        protected async UniTask WaitUntilHoldTime()
        {
            await UniTask.Delay(millisecondsToHold);
            if (HoldingTimeInSeconds == null
                || HoldingTimeInSeconds + errorHoldTimeInMilliseconds < millisecondsToHold / 1000f)
                return;
            OnHold();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable)
                return;
            if (_pressedTime == null)
                return;
            base.OnPointerUp(eventData);
            _pressedTime = null;
            OnClick();
            Debug.Log("Released");
        }

        //TODO make private
        public void OnClick()
        {
            Debug.Log("Clicked");
            Clicked?.Invoke(this);
            _abilityView.Clicked();
        }

        public void OnHold()
        {
            Debug.Log("Holded for" + HoldingTimeInSeconds);
            _pressedTime = null;
        }

        public void SetAbility(AbilityView abilityView)
        {
            RemoveAbility();
            _abilityName.text = abilityView.Name;
            _abilityImage.sprite = abilityView.AbilityIcon;
            _abilityView = abilityView;
            _abilityView.Casted += OnCasted;
            CheckUsePossibility();
        }

        public void CancelAbilityCast() => _abilityView?.CancelCast();
        
        private void OnCasted() => Casted?.Invoke();

        public void CheckUsePossibility()
        {
            if(_abilityView.CanCast)
            {
                interactable = true;
                return;
            }
            interactable = false;
        }

        public void RemoveAbility()
        {
            CancelAbilityCast();
            _abilityImage.sprite = null;
            _abilityName.text = "";
            if(_abilityView != null)
                _abilityView.Casted -= OnCasted;
            _abilityView = null;
            interactable = false;
            onClick.RemoveAllListeners();
        }
    }
}