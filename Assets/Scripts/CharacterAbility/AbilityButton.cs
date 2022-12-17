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
    public class AbilityButton : Button
    {
        public event Action AbilityButtonUsed;

        public AbilityView AbilityView { get; private set; }
        public event Action<AbilityButton> Clicked;
        public event Action<AbilityButton> Holded;
        [SerializeField]
        private Image _abilityImage;
        [SerializeField]
        private TextMeshProUGUI _abilityName;
        private static readonly int millisecondsToHold = 700;
        private static readonly int errorHoldTimeInMilliseconds = Math.Min(10, millisecondsToHold);
        private float? _pressedTime;
        private float? _releasedTime;
        
        public float? HoldingTimeInSeconds
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

        private void Awake()
        {
            interactable = false;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
                return;
            _pressedTime = Time.time;
            _releasedTime = null;
            base.OnPointerDown(eventData);
            UniTask.Create(WaitUntilHoldTime);
            Debug.Log("Pressed");
        }

        protected async UniTask WaitUntilHoldTime()
        {
            await UniTask.Delay(millisecondsToHold, ignoreTimeScale: true);
            _releasedTime = Time.time;
            if (!HoldingTimeInSeconds.HasValue
                || HoldingTimeInSeconds.Value + errorHoldTimeInMilliseconds / 1000f < millisecondsToHold / 1000f)
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

        //TODO make private
        public void OnClick()
        {
            _releasedTime = Time.time;
            Clicked?.Invoke(this);
            //TODO Логика зависит от UI
            //if (AbilityView.CanCast)
                AbilityView.Clicked();
            Debug.Log("Clicked");
        }

        public void OnHold()
        {
            Debug.Log("Holded for " + HoldingTimeInSeconds.Value);
            Holded?.Invoke(this);
            OnPointerUp(new PointerEventData(EventSystem.current));
        }

        public AbilityInfo GetAbilityInfo() => AbilityView.AbilityInfo;

        public void SetAbility(AbilityView abilityView)
        {
            RemoveAbility();
            interactable = true;
            _abilityName.text = abilityView.Name;
            _abilityImage.sprite = abilityView.AbilityIcon;
            AbilityView = abilityView;
            AbilityView.Casted += OnAbilityCasted;
        }

        public void CancelAbilityCast() => AbilityView?.CancelCast();
        
        private void OnAbilityCasted() => AbilityButtonUsed?.Invoke();

        public void RemoveAbility()
        {
            interactable = false;
            CancelAbilityCast();
            _abilityImage.sprite = null;
            _abilityName.text = "";
            if(AbilityView != null)
                AbilityView.Casted -= OnAbilityCasted;
            AbilityView = null;
            onClick.RemoveAllListeners();
        }
    }
}