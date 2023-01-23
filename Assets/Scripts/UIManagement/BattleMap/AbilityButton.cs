using CharacterAbility;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [RequireComponent(typeof(HoldableButton)), DisallowMultipleComponent]
    public class AbilityButton : MonoBehaviour
    {
        [SerializeField]
        private Image _abilityIcon;
        [SerializeField]
        private TextMeshProUGUI _abilityName;
        [SerializeField]
        private HoldableButton _button;
        public HoldableButton HoldableButton => _button;

        private Sprite _noSelectedAbilityIcon;
        public Sprite NoSelectedAbilityIcon
        {
            get => _noSelectedAbilityIcon;
            set
            {
                _noSelectedAbilityIcon = value;
                if (_abilityIcon.sprite != value)
                    _abilityIcon.sprite = value;
            }
        }
        public AbilityView AbilityView { get; private set; }
        public event Action<AbilityButton> Clicked;
        public event Action<AbilityButton> Holded;
        public event Action AbilityButtonUsed;

        private void Awake()
        {
            RemoveAbilityView();
            _button.Clicked += OnClick;
            _button.Holded += OnHold;
        }

        private void OnClick(HoldableButton button)
        {
            Clicked?.Invoke(this);
            Select();
        }

        private void OnHold(HoldableButton button, float holdTime) => Holded?.Invoke(this);

        private void OnAbilityCasted(ActionType _) => AbilityButtonUsed?.Invoke();

        public void Select()
        {
            //TODO Логика зависит от UI
            //if (AbilityView.CanCast)
            AbilityView.Clicked();
        }

        public void AssignAbilityView(AbilityView abilityView)
        {
            RemoveAbilityView();
            _abilityName.text = abilityView.Name;
            _abilityIcon.sprite = abilityView.AbilityIcon;
            AbilityView = abilityView;
            AbilityView.Casted += OnAbilityCasted;
            _button.interactable = true;
            UpdateAvailability();
        }

        public void CancelAbilityCast() => AbilityView?.CancelCast();

        public void RemoveAbilityView()
        {
            CancelAbilityCast();
            _abilityIcon.sprite = NoSelectedAbilityIcon;
            _abilityName.text = "";
            if(AbilityView != null)
                AbilityView.Casted -= OnAbilityCasted;
            AbilityView = null;
            _button.interactable = false;
        }

        public void UpdateAvailability()
        {
            _button.ClickAvailable = AbilityView.CanCast;
        }
    }
}