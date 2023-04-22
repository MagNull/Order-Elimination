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
        public CharacterAbility.AbilityView AbilityView { get; private set; }
        public OrderElimination.AbilitySystem.AbilityRunner AbilityRunner { get; private set; }
        public event Action<AbilityButton> Clicked;
        public event Action<AbilityButton> Holded;

        private void Awake()
        {
            RemoveAbility();
            _button.Clicked += OnClick;
            _button.Holded += OnHold;
        }

        private void OnClick(HoldableButton button)
        {
            Clicked?.Invoke(this);
        }

        private void OnHold(HoldableButton button, float holdTime) => Holded?.Invoke(this);

        public void AssignAbiility(OrderElimination.AbilitySystem.AbilityRunner abilityRunner)
        {
            RemoveAbility();
            AbilityRunner = abilityRunner;
            _abilityName.text = AbilityRunner.AbilityData.View.Name;
            _abilityIcon.sprite = AbilityRunner.AbilityData.View.Icon;
            _button.interactable = true;
        }

        public void RemoveAbility()
        {
            _abilityIcon.sprite = NoSelectedAbilityIcon;
            HoldableButton.SetImageTint(Color.white);
            _abilityName.text = "";
            AbilityRunner = null;
            _button.interactable = false;
        }

        public void SetClickAvailability(bool isClickAvailable)
        {
            _button.ClickAvailable = isClickAvailable;
        }
    }
}