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
        public event Action AbilityButtonUsed;

        public AbilityView AbilityView { get; private set; }
        public event Action<AbilityButton> Clicked;
        public event Action<AbilityButton> Holded;
        [SerializeField]
        private Image _abilityImage;
        [SerializeField]
        private TextMeshProUGUI _abilityName;
        [SerializeField]
        private HoldableButton _button;
        public HoldableButton HoldableButton => _button;

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

        private void OnAbilityCasted() => AbilityButtonUsed?.Invoke();

        public void Select()
        {
            //TODO Логика зависит от UI
            //if (AbilityView.CanCast)
            AbilityView.Clicked();
            Debug.Log("Clicked");
        }

        public void AssignAbilityView(AbilityView abilityView)
        {
            RemoveAbilityView();
            _abilityName.text = abilityView.Name;
            _abilityImage.sprite = abilityView.AbilityIcon;
            AbilityView = abilityView;
            AbilityView.Casted += OnAbilityCasted;
            _button.interactable = true;
            UpdateAvailability();
        }

        public void CancelAbilityCast() => AbilityView?.CancelCast();

        public void RemoveAbilityView()
        {
            CancelAbilityCast();
            _abilityImage.sprite = null;
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