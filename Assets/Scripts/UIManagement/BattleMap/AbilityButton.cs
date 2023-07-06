using DG.Tweening;
using OrderElimination.AbilitySystem;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [RequireComponent(typeof(HoldableButton)), DisallowMultipleComponent]
    public class AbilityButton : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private Image _abilityIcon;
        [SerializeField]
        private TextMeshProUGUI _abilityName;
        [SerializeField]
        private HoldableButton _button;
        [field: SerializeField]
        public CooldownTimer CooldownTimer { get; private set; }
        public HoldableButton HoldableButton => _button;
        [field: SerializeField]
        private Sprite _noSelectedAbilityIcon;
        public ActiveAbilityRunner AbilityRunner { get; private set; }

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

        public void AssignAbiility(ActiveAbilityRunner abilityRunner)
        {
            RemoveAbility();
            AbilityRunner = abilityRunner;
            _abilityName.text = AbilityRunner.AbilityData.View.Name;
            _abilityIcon.sprite = AbilityRunner.AbilityData.View.Icon;
            _button.interactable = true;
            CooldownTimer.DOComplete();
            CooldownTimer.gameObject.SetActive(true);
        }

        public void RemoveAbility()
        {
            _abilityIcon.sprite = _noSelectedAbilityIcon;
            HoldableButton.SetImageTint(Color.white);
            _abilityName.text = "";
            AbilityRunner = null;
            _button.interactable = false;
            CooldownTimer.SetValue(0);
            CooldownTimer.DOComplete();
            CooldownTimer.gameObject.SetActive(false);
        }

        public void SetClickAvailability(bool isClickAvailable)
        {
            _button.ClickAvailable = isClickAvailable;
        }
    }
}